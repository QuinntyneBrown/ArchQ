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
        var cluster = await _context.GetClusterAsync();
        var queryOptions = new QueryOptions().Timeout(TimeSpan.FromSeconds(10));

        // Allow collections to stabilize before creating indexes
        await Task.Delay(2000);

        // Primary indexes on all collections using fully qualified keyspace paths
        foreach (var name in CollectionNames)
        {
            var query = $"CREATE PRIMARY INDEX IF NOT EXISTS ON `archq`.`{slug}`.`{name}`";
            using var result = await cluster.QueryAsync<dynamic>(query, queryOptions);
            // Consume the result to ensure the query completes and the connection is released
            await foreach (var _ in result) { }
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
            var query = $"CREATE INDEX `{indexName}` IF NOT EXISTS ON `archq`.`{slug}`.`{collection}`(`{field}`)";
            using var result = await cluster.QueryAsync<dynamic>(query, queryOptions);
            await foreach (var _ in result) { }
        }
    }
}
