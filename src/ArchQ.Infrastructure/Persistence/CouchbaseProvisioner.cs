using ArchQ.Core.Interfaces;
using Couchbase.Management.Collections;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence;

public class CouchbaseProvisioner : ICouchbaseProvisioner
{
    private readonly CouchbaseContext _context;

    private static readonly string[] CollectionNames =
    [
        "users",
        "adrs",
        "adr_versions",
        "meeting_notes",
        "notes",
        "comments",
        "attachments_meta",
        "audit",
        "tags",
        "config"
    ];

    public CouchbaseProvisioner(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task ProvisionScopeAsync(string slug)
    {
        var bucket = await _context.GetBucketAsync();
        var collectionManager = bucket.Collections;

        try
        {
            await collectionManager.CreateScopeAsync(slug);
        }
        catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
        {
            // Scope already exists, continue
        }
    }

    public async Task CreateCollectionsAsync(string slug)
    {
        var bucket = await _context.GetBucketAsync();
        var collectionManager = bucket.Collections;

        foreach (var name in CollectionNames)
        {
            try
            {
                await collectionManager.CreateCollectionAsync(slug, name, new CreateCollectionSettings());
            }
            catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                // Collection already exists, continue
            }
        }
    }

    public async Task CreateIndexesAsync(string slug)
    {
        var bucket = await _context.GetBucketAsync();
        var scope = bucket.Scope(slug);

        // Primary indexes on all collections
        foreach (var name in CollectionNames)
        {
            var query = $"CREATE PRIMARY INDEX IF NOT EXISTS ON `{name}`";
            await scope.QueryAsync<dynamic>(query, new QueryOptions());
        }

        // Secondary indexes per collection
        var secondaryIndexes = new (string Collection, string IndexName, string Field)[]
        {
            ("users", "idx_users_email", "email"),
            ("adrs", "idx_adrs_status", "status"),
            ("adrs", "idx_adrs_createdAt", "createdAt"),
            ("tags", "idx_tags_name", "name"),
            ("audit", "idx_audit_timestamp", "timestamp"),
            ("comments", "idx_comments_adrId", "adrId"),
        };

        foreach (var (collection, indexName, field) in secondaryIndexes)
        {
            var query = $"CREATE INDEX `{indexName}` IF NOT EXISTS ON `{collection}`(`{field}`)";
            await scope.QueryAsync<dynamic>(query, new QueryOptions());
        }
    }
}
