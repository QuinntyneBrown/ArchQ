using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class ConfigRepository : IConfigRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "config";

    public ConfigRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string key) => $"config::{key}";

    public async Task<TemplateConfig?> GetByKeyAsync(string key, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(DocKey(key));
            return result.ContentAs<TemplateConfig>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task UpsertAsync(TemplateConfig config, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.UpsertAsync(DocKey(config.Key), config);
    }
}
