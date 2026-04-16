using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class TagRepository : ITagRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "tags";

    public TagRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string slug) => Tag.GenerateId(slug);

    public async Task<Tag> CreateAsync(Tag tag, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(DocKey(tag.Slug), tag);
        return tag;
    }

    public async Task<Tag?> GetBySlugAsync(string slug, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(DocKey(slug));
            return result.ContentAs<Tag>();
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<Tag>> ListAllAsync(string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT t.* FROM `{CollectionName}` t WHERE t.type = \"tag\" ORDER BY t.usageCount DESC, t.name ASC";
        var result = await scope.QueryAsync<Tag>(query);
        var tags = new List<Tag>();
        await foreach (var row in result.Rows)
        {
            tags.Add(row);
        }
        return tags;
    }

    public async Task<List<Tag>> SearchAsync(string queryText, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT t.* FROM `{CollectionName}` t WHERE t.type = \"tag\" AND LOWER(t.name) LIKE $prefix ORDER BY t.usageCount DESC, t.name ASC LIMIT 20";
        var options = new QueryOptions().Parameter("prefix", queryText.ToLowerInvariant() + "%");
        var result = await scope.QueryAsync<Tag>(query, options);
        var tags = new List<Tag>();
        await foreach (var row in result.Rows)
        {
            tags.Add(row);
        }
        return tags;
    }

    public async Task IncrementUsageAsync(string slug, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var getResult = await collection.GetAsync(DocKey(slug));
            var tag = getResult.ContentAs<Tag>();
            if (tag != null)
            {
                tag.UsageCount++;
                await collection.ReplaceAsync(DocKey(slug), tag, new Couchbase.KeyValue.ReplaceOptions().Cas(getResult.Cas));
            }
        }
        catch (DocumentNotFoundException)
        {
            // Tag doesn't exist — nothing to increment
        }
    }

    public async Task DecrementUsageAsync(string slug, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var getResult = await collection.GetAsync(DocKey(slug));
            var tag = getResult.ContentAs<Tag>();
            if (tag != null && tag.UsageCount > 0)
            {
                tag.UsageCount--;
                await collection.ReplaceAsync(DocKey(slug), tag, new Couchbase.KeyValue.ReplaceOptions().Cas(getResult.Cas));
            }
        }
        catch (DocumentNotFoundException)
        {
            // Tag doesn't exist — nothing to decrement
        }
    }
}
