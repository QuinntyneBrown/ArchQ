using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;

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
}
