using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AdrVersionRepository : IAdrVersionRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "adr_versions";

    public AdrVersionRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string id) => id;

    public async Task<AdrVersion> CreateAsync(AdrVersion version, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(DocKey(version.Id), version);
        return version;
    }

    public async Task<List<AdrVersion>> ListByAdrAsync(string adrId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT v.* FROM `{CollectionName}` v WHERE v.type = \"adr_version\" AND v.adrId = $adrId ORDER BY v.version DESC";
        var options = new QueryOptions().Parameter("adrId", adrId);
        var result = await scope.QueryAsync<AdrVersion>(query, options);
        var versions = new List<AdrVersion>();
        await foreach (var row in result.Rows)
        {
            versions.Add(row);
        }
        return versions;
    }

    public async Task<AdrVersion?> GetByVersionAsync(string adrId, int version, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT v.* FROM `{CollectionName}` v WHERE v.type = \"adr_version\" AND v.adrId = $adrId AND v.version = $version LIMIT 1";
        var options = new QueryOptions()
            .Parameter("adrId", adrId)
            .Parameter("version", version);
        var result = await scope.QueryAsync<AdrVersion>(query, options);
        await foreach (var row in result.Rows)
        {
            return row;
        }
        return null;
    }
}
