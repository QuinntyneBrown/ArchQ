using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "_system";
    private const string CollectionName = "refresh_tokens";

    public RefreshTokenRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string tokenHash) => $"rtoken::{tokenHash}";

    private async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        return await _context.GetCollectionAsync(ScopeName, CollectionName);
    }

    public async Task CreateAsync(RefreshToken refreshToken)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(DocKey(refreshToken.TokenHash), refreshToken);
    }

    public async Task<RefreshToken?> GetByTokenHashAsync(string hash)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(DocKey(hash));
            return result.ContentAs<RefreshToken>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task UpdateAsync(RefreshToken refreshToken)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(DocKey(refreshToken.TokenHash), refreshToken);
    }

    public async Task RevokeAllInFamilyAsync(string family)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var query = $"SELECT META().id AS docId, t.* FROM `{CollectionName}` t WHERE t.family = $family AND t.revoked = false";
        var queryOptions = new QueryOptions().Parameter("family", family);
        var result = await scope.QueryAsync<dynamic>(query, queryOptions);

        var collection = await GetCollectionAsync();

        await foreach (var row in result.Rows)
        {
            string docId = row.docId;
            try
            {
                await collection.MutateInAsync(docId, specs =>
                    specs.Replace("revoked", true)
                         .Replace("revokedAt", DateTime.UtcNow.ToString("o")));
            }
            catch
            {
                // Best effort — token may have been concurrently modified
            }
        }
    }
}
