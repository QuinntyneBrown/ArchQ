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
}
