using Couchbase.Management.Collections;

namespace ArchQ.Infrastructure.Persistence;

public class CouchbaseBootstrapper
{
    private readonly CouchbaseContext _context;

    private const string SystemScope = "_system";

    private static readonly string[] SystemCollections =
    [
        "tenants",
        "global_users",
        "verification_tokens"
    ];

    public CouchbaseBootstrapper(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task EnsureSystemScopeAsync()
    {
        var bucket = await _context.GetBucketAsync();
        var collectionManager = bucket.Collections;

        try
        {
            await collectionManager.CreateScopeAsync(SystemScope);
        }
        catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            // Scope already exists, continue
        }

        foreach (var name in SystemCollections)
        {
            try
            {
                await collectionManager.CreateCollectionAsync(SystemScope, name, new CreateCollectionSettings());
            }
            catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                // Collection already exists, continue
            }
        }
    }
}
