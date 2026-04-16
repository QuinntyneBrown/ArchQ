using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using Couchbase.Management.Collections;
using Couchbase.Query;
using Microsoft.Extensions.Options;

namespace ArchQ.PerformanceTests.Infrastructure;

/// <summary>
/// Manages a real Couchbase connection for benchmarks.
/// Provisioning (scope/collection/index creation) is done once in Program.cs
/// before BenchmarkDotNet launches child processes. Each child process
/// just connects to the already-provisioned scope.
/// </summary>
public sealed class CouchbaseFixture : IAsyncDisposable
{
    private static readonly Lazy<CouchbaseFixture> Instance = new(() => new CouchbaseFixture());

    public static CouchbaseFixture Shared => Instance.Value;

    public CouchbaseContext Context { get; }
    public string TenantSlug { get; } = "bench_perf";

    private static readonly string[] TenantCollections =
    [
        "users", "adrs", "adr_versions", "meeting_notes", "notes",
        "comments", "attachments_meta", "audit", "tags", "config"
    ];

    private static readonly string[] SystemCollections = ["audit", "tenants", "global_users"];

    private CouchbaseFixture()
    {
        var config = new CouchbaseConfiguration
        {
            ConnectionString = Environment.GetEnvironmentVariable("COUCHBASE_CONNECTION_STRING") ?? "couchbase://localhost",
            Username = Environment.GetEnvironmentVariable("COUCHBASE_USERNAME") ?? "Developer",
            Password = Environment.GetEnvironmentVariable("COUCHBASE_PASSWORD") ?? "password",
            BucketName = Environment.GetEnvironmentVariable("COUCHBASE_BUCKET") ?? "archq"
        };

        Context = new CouchbaseContext(Options.Create(config));
    }

    /// <summary>
    /// Called ONCE in Program.cs (main process) before BenchmarkDotNet starts.
    /// Creates scope, collections, and indexes. Must complete before any child process runs.
    /// </summary>
    public async Task ProvisionAsync()
    {
        var bucket = await Context.GetBucketAsync();
        var mgr = bucket.Collections;

        // Create tenant scope + collections
        await CreateScopeAndCollections(mgr, TenantSlug, TenantCollections);

        // Create system scope for AuditRepository
        await CreateScopeAndCollections(mgr, "system", SystemCollections);

        // Wait for collections to be queryable
        await Task.Delay(5000);

        // Create indexes on tenant scope — one at a time, with retry
        var tenantScope = bucket.Scope(TenantSlug);
        foreach (var name in TenantCollections)
        {
            await CreateIndexWithRetry(tenantScope, $"CREATE PRIMARY INDEX IF NOT EXISTS ON `{name}`");
        }

        var secondaryIndexes = new (string Collection, string IndexName, string Field)[]
        {
            ("users", "idx_bench_users_email", "email"),
            ("adrs", "idx_bench_adrs_status", "status"),
            ("adrs", "idx_bench_adrs_createdAt", "createdAt"),
            ("tags", "idx_bench_tags_name", "name"),
            ("audit", "idx_bench_audit_timestamp", "timestamp"),
            ("comments", "idx_bench_comments_adrId", "adrId"),
        };

        foreach (var (collection, indexName, field) in secondaryIndexes)
        {
            await CreateIndexWithRetry(tenantScope, $"CREATE INDEX `{indexName}` IF NOT EXISTS ON `{collection}`(`{field}`)");
        }

        // Create indexes on _system scope
        var systemScope = bucket.Scope("system");
        await CreateIndexWithRetry(systemScope, "CREATE PRIMARY INDEX IF NOT EXISTS ON `audit`");
        await CreateIndexWithRetry(systemScope, "CREATE INDEX `idx_bench_sys_audit_ts` IF NOT EXISTS ON `audit`(`timestamp`)");

        // Wait for all indexes to finish building
        await Task.Delay(10000);
    }

    private static async Task CreateScopeAndCollections(
        ICouchbaseCollectionManager mgr, string scopeName, IEnumerable<string> collections)
    {
        try { await mgr.CreateScopeAsync(scopeName); }
        catch { /* scope may already exist */ }

        foreach (var name in collections)
        {
            try { await mgr.CreateCollectionAsync(scopeName, name, new CreateCollectionSettings()); }
            catch { /* collection may already exist */ }
        }
    }

    private static async Task CreateIndexWithRetry(Couchbase.KeyValue.IScope scope, string statement)
    {
        for (int attempt = 0; attempt < 10; attempt++)
        {
            try
            {
                var options = new QueryOptions().Timeout(TimeSpan.FromSeconds(60));
                await scope.QueryAsync<dynamic>(statement, options);
                return;
            }
            catch (Exception ex) when (
                ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase)
                || ex.Message.Contains("duplicate", StringComparison.OrdinalIgnoreCase)
                || ex.InnerException?.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase) == true)
            {
                return; // Index exists — done
            }
            catch when (attempt < 9)
            {
                // Transient error (build in progress, timeout, etc.) — wait and retry
                await Task.Delay(5000 * (attempt + 1));
            }
        }
    }

    public async ValueTask DisposeAsync()
    {
        try
        {
            var bucket = await Context.GetBucketAsync();
            try { await bucket.Collections.DropScopeAsync(TenantSlug); } catch { }
            try { await bucket.Collections.DropScopeAsync("system"); } catch { }
        }
        catch { /* best-effort cleanup */ }

        await Context.DisposeAsync();
    }
}
