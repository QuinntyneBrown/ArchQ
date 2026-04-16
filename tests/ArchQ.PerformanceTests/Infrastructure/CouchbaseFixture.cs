using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using Couchbase.Management.Collections;
using Couchbase.Query;
using Microsoft.Extensions.Options;

namespace ArchQ.PerformanceTests.Infrastructure;

/// <summary>
/// Manages a real Couchbase connection and a dedicated benchmark scope
/// that is provisioned once and torn down after all benchmarks complete.
/// </summary>
public sealed class CouchbaseFixture : IAsyncDisposable
{
    private static readonly Lazy<CouchbaseFixture> Instance = new(() => new CouchbaseFixture());

    public static CouchbaseFixture Shared => Instance.Value;

    public CouchbaseContext Context { get; }
    public string TenantSlug { get; } = $"bench_{DateTime.UtcNow:yyyyMMddHHmmss}";

    private static readonly string[] CollectionNames =
    [
        "users", "adrs", "adr_versions", "meeting_notes", "notes",
        "comments", "attachments_meta", "audit", "tags", "config"
    ];

    private bool _provisioned;

    private CouchbaseFixture()
    {
        var config = new CouchbaseConfiguration
        {
            ConnectionString = Environment.GetEnvironmentVariable("COUCHBASE_CONNECTION_STRING") ?? "couchbase://localhost",
            Username = Environment.GetEnvironmentVariable("COUCHBASE_USERNAME") ?? "Administrator",
            Password = Environment.GetEnvironmentVariable("COUCHBASE_PASSWORD") ?? "password",
            BucketName = Environment.GetEnvironmentVariable("COUCHBASE_BUCKET") ?? "archq"
        };

        Context = new CouchbaseContext(Options.Create(config));
    }

    public async Task EnsureProvisionedAsync()
    {
        if (_provisioned) return;

        var bucket = await Context.GetBucketAsync();
        var collectionManager = bucket.Collections;

        try { await collectionManager.CreateScopeAsync(TenantSlug); }
        catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)) { }

        foreach (var name in CollectionNames)
        {
            try { await collectionManager.CreateCollectionAsync(TenantSlug, name, new CreateCollectionSettings()); }
            catch (Exception ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)) { }
        }

        // Wait for collections to be ready
        await Task.Delay(2000);

        var scope = bucket.Scope(TenantSlug);
        foreach (var name in CollectionNames)
        {
            var query = $"CREATE PRIMARY INDEX IF NOT EXISTS ON `{name}`";
            await scope.QueryAsync<dynamic>(query, new QueryOptions());
        }

        // Secondary indexes needed by benchmarks
        var indexes = new (string Collection, string IndexName, string Field)[]
        {
            ("users", "idx_bench_users_email", "email"),
            ("adrs", "idx_bench_adrs_status", "status"),
            ("adrs", "idx_bench_adrs_createdAt", "createdAt"),
            ("tags", "idx_bench_tags_name", "name"),
            ("audit", "idx_bench_audit_timestamp", "timestamp"),
            ("comments", "idx_bench_comments_adrId", "adrId"),
        };

        foreach (var (collection, indexName, field) in indexes)
        {
            var q = $"CREATE INDEX `{indexName}` IF NOT EXISTS ON `{collection}`(`{field}`)";
            await scope.QueryAsync<dynamic>(q, new QueryOptions());
        }

        // Wait for indexes to build
        await Task.Delay(3000);

        _provisioned = true;
    }

    public async ValueTask DisposeAsync()
    {
        // Drop the benchmark scope to clean up
        try
        {
            var bucket = await Context.GetBucketAsync();
            await bucket.Collections.DropScopeAsync(TenantSlug);
        }
        catch { /* best-effort cleanup */ }

        await Context.DisposeAsync();
    }
}
