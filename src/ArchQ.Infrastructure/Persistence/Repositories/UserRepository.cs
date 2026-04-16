using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "users";

    public UserRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string id) => $"user::{id}";

    private async Task<ICouchbaseCollection> GetCollectionAsync(string tenantSlug)
    {
        return await _context.GetCollectionAsync(tenantSlug, CollectionName);
    }

    public async Task<User> CreateAsync(User user, string tenantSlug)
    {
        var collection = await GetCollectionAsync(tenantSlug);
        await collection.InsertAsync(DocKey(user.Id), user);
        return user;
    }

    public async Task<User?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await GetCollectionAsync(tenantSlug);
        try
        {
            var result = await collection.GetAsync(DocKey(id));
            return result.ContentAs<User>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<User?> GetByEmailAsync(string email, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT u.* FROM `{CollectionName}` u WHERE u.email = $email LIMIT 1";
        var queryOptions = new QueryOptions().Parameter("email", email);
        var result = await scope.QueryAsync<User>(query, queryOptions);

        await foreach (var row in result.Rows)
        {
            return row;
        }

        return null;
    }

    public async Task<User> UpdateAsync(User user, string tenantSlug)
    {
        var collection = await GetCollectionAsync(tenantSlug);
        user.UpdatedAt = DateTime.UtcNow;
        await collection.ReplaceAsync(DocKey(user.Id), user);
        return user;
    }

    public async Task<Dictionary<string, User>> GetByIdsAsync(List<string> ids, string tenantSlug)
    {
        var result = new Dictionary<string, User>();
        if (ids.Count == 0) return result;

        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT u.* FROM `{CollectionName}` u WHERE u.id IN $ids";
        var queryOptions = new QueryOptions().Parameter("ids", ids);
        var queryResult = await scope.QueryAsync<User>(query, queryOptions);

        await foreach (var row in queryResult.Rows)
        {
            if (row != null && !string.IsNullOrEmpty(row.Id))
            {
                result[row.Id] = row;
            }
        }

        return result;
    }

    public async Task<int> CountByRoleAsync(string role, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT COUNT(*) AS cnt FROM `{CollectionName}` u WHERE ANY r IN u.roles SATISFIES r = $role END";
        var queryOptions = new QueryOptions().Parameter("role", role);
        var result = await scope.QueryAsync<dynamic>(query, queryOptions);

        await foreach (var row in result.Rows)
        {
            return (int)row.cnt;
        }

        return 0;
    }
}
