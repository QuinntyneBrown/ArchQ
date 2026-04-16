using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AdrRepository : IAdrRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "adrs";
    private const int MaxCasRetries = 3;

    public AdrRepository(CouchbaseContext context)
    {
        _context = context;
    }

    // Id already carries the "adr::" prefix (e.g. "adr::550e8400-...")
    private static string DocKey(string id) => id;

    public async Task<Adr> CreateAsync(Adr adr, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);

        for (int attempt = 0; attempt <= MaxCasRetries; attempt++)
        {
            try
            {
                await collection.InsertAsync(DocKey(adr.Id), adr);
                return adr;
            }
            catch (DocumentExistsException) when (attempt < MaxCasRetries)
            {
                // Concurrent insert conflict — re-generate id and re-sequence the ADR number
                adr.Id = $"adr::{Guid.NewGuid()}";
                var maxNum = await GetMaxAdrNumberAsync(tenantSlug);
                adr.AdrNumber = $"ADR-{(maxNum + 1):D3}";
            }
        }

        throw new InvalidOperationException("Failed to insert ADR after maximum CAS retries due to concurrent creation conflicts.");
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
