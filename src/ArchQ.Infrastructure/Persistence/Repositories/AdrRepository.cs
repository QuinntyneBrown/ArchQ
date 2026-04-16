using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AdrRepository : IAdrRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "adrs";

    public AdrRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string id) => $"adr::{id}";

    public async Task<Adr> CreateAsync(Adr adr, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(DocKey(adr.Id), adr);
        return adr;
    }

    public async Task<Adr?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(DocKey(id));
            return result.ContentAs<Adr>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<int> GetMaxAdrNumberAsync(string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT MAX(TONUMBER(REPLACE(a.adrNumber, \"ADR-\", \"\"))) AS maxNum FROM `{CollectionName}` a WHERE a.type = \"adr\"";
        var result = await scope.QueryAsync<dynamic>(query);

        await foreach (var row in result.Rows)
        {
            if (row.maxNum is not null)
            {
                return (int)(long)row.maxNum;
            }
        }

        return 0;
    }
}
