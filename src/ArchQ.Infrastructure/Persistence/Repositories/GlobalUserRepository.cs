using System.Security.Cryptography;
using System.Text;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.KeyValue;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class GlobalUserRepository : IGlobalUserRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "_system";
    private const string CollectionName = "global_users";

    public GlobalUserRepository(CouchbaseContext context)
    {
        _context = context;
    }

    private static string DocKey(string email)
    {
        var hash = SHA256.HashData(Encoding.UTF8.GetBytes(email.ToLowerInvariant()));
        return $"guser::{Convert.ToHexStringLower(hash)}";
    }

    private async Task<ICouchbaseCollection> GetCollectionAsync()
    {
        return await _context.GetCollectionAsync(ScopeName, CollectionName);
    }

    public async Task<GlobalUser?> GetByEmailAsync(string email)
    {
        var collection = await GetCollectionAsync();
        try
        {
            var result = await collection.GetAsync(DocKey(email));
            return result.ContentAs<GlobalUser>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<GlobalUser> CreateAsync(GlobalUser globalUser)
    {
        var collection = await GetCollectionAsync();
        await collection.InsertAsync(DocKey(globalUser.Email), globalUser);
        return globalUser;
    }

    public async Task<GlobalUser> UpdateAsync(GlobalUser globalUser)
    {
        var collection = await GetCollectionAsync();
        await collection.ReplaceAsync(DocKey(globalUser.Email), globalUser);
        return globalUser;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        var collection = await GetCollectionAsync();
        try
        {
            await collection.GetAsync(DocKey(email));
            return true;
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return false;
        }
    }
}
