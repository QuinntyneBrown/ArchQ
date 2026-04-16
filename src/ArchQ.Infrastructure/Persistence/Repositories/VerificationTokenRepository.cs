using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class VerificationTokenRepository : IVerificationTokenRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "_system";
    private const string CollectionName = "verification_tokens";

    public VerificationTokenRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string tokenHash) => $"vtoken::{tokenHash}";

    private async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        return await _context.GetCollectionAsync(ScopeName, CollectionName);
    }

    public async Task CreateAsync(VerificationToken token)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(DocKey(token.TokenHash), token);
    }

    public async Task<VerificationToken?> GetByTokenHashAsync(string tokenHash)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(DocKey(tokenHash));
            return result.ContentAs<VerificationToken>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task UpdateAsync(VerificationToken token)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(DocKey(token.TokenHash), token);
    }
}
