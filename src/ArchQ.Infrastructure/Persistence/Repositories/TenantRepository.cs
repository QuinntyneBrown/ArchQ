using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class TenantRepository : ITenantRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "_system";
    private const string CollectionName = "tenants";

    public TenantRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string id) => $"tenant::{id}";

    private async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        return await _context.GetCollectionAsync(ScopeName, CollectionName);
    }

    public async Task<Tenant?> GetByIdAsync(string id)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(DocKey(id));
            return result.ContentAs<Tenant>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<Tenant?> GetBySlugAsync(string slug)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var bucket = await _context.GetBucketAsync();
        var query = $"SELECT t.* FROM `{CollectionName}` t WHERE t.slug = $slug LIMIT 1";
        var queryOptions = new QueryOptions().Parameter("slug", slug);
        var result = await scope.QueryAsync<Tenant>(query, queryOptions);

        await foreach (var row in result.Rows)
        {
            return row;
        }

        return null;
    }

    public async Task<Tenant> CreateAsync(Tenant tenant)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(DocKey(tenant.Id), tenant);
        return tenant;
    }

    public async Task<Tenant> UpdateAsync(Tenant tenant)
    {
        var collection = await GetCollectionAsync();
        tenant.UpdatedAt = DateTime.UtcNow;
        await collection.ReplaceAsync(DocKey(tenant.Id), tenant);
        return tenant;
    }

    public async Task SoftDeleteAsync(string id)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(DocKey(id));
            var tenant = result.ContentAs<Tenant>()
                ?? throw new Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException();
            tenant.Status = "deleted";
            tenant.UpdatedAt = DateTime.UtcNow;
            await collection.ReplaceAsync(DocKey(id), tenant);
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            throw;
        }
    }

    public async Task<bool> SlugExistsAsync(string slug)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var query = $"SELECT COUNT(*) as cnt FROM `{CollectionName}` WHERE slug = $slug";
        var queryOptions = new QueryOptions().Parameter("slug", slug);
        var result = await scope.QueryAsync<dynamic>(query, queryOptions);

        await foreach (var row in result.Rows)
        {
            long count = (long)row.cnt;
            return count > 0;
        }

        return false;
    }
}
