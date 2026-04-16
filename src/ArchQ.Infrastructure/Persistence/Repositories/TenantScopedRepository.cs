using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public abstract class TenantScopedRepository<T> where T : class
{
    private readonly CouchbaseContext _context;
    private readonly ITenantContext _tenantContext;
    private readonly string _collectionName;

    protected TenantScopedRepository(CouchbaseContext context, ITenantContext tenantContext, string collectionName)
    {
        _context = context;
        _tenantContext = tenantContext;
        _collectionName = collectionName;
    }

    protected string TenantSlug => _tenantContext.TenantSlug;

    protected async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        var bucket = await _context.GetBucketAsync();
        return bucket.Scope(_tenantContext.TenantSlug).Collection(_collectionName);
    }

    public virtual async Task<T?> GetByIdAsync(string id)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(id);
            return result.ContentAs<T>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public virtual async Task<IEnumerable<T>> QueryAsync(string whereClause, IDictionary<string, object>? parameters = null)
    {
        var bucket = await _context.GetBucketAsync();
        var scope = bucket.Scope(_tenantContext.TenantSlug);
        var query = $"SELECT d.* FROM `{_collectionName}` d WHERE {whereClause}";
        var queryOptions = new QueryOptions();

        if (parameters != null)
        {
            foreach (var kvp in parameters)
            {
                queryOptions.Parameter(kvp.Key, kvp.Value);
            }
        }

        var result = await scope.QueryAsync<T>(query, queryOptions);
        var items = new List<T>();

        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }

        return items;
    }

    public virtual async Task UpsertAsync(string id, T document)
    {
        var collection = await GetCollectionAsync();
        await collection.UpsertAsync(id, document);
    }

    public virtual async Task RemoveAsync(string id)
    {
        var collection = await GetCollectionAsync();
        await collection.RemoveAsync(id);
    }
}
