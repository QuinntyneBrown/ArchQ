using BenchmarkDotNet.Attributes;

namespace ArchQ.PerformanceTests.Infrastructure;

/// <summary>
/// Base class for all Couchbase benchmarks. Handles one-time scope provisioning
/// via GlobalSetup/GlobalCleanup so each benchmark class gets a ready-to-use
/// connection against a real Couchbase database.
/// </summary>
public abstract class CouchbaseBenchmarkBase
{
    protected CouchbaseFixture Fixture => CouchbaseFixture.Shared;
    protected string TenantSlug => Fixture.TenantSlug;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        await Fixture.EnsureProvisionedAsync();
        await SetupAsync();
    }

    [GlobalCleanup]
    public async Task GlobalCleanup()
    {
        await CleanupAsync();
    }

    protected virtual Task SetupAsync() => Task.CompletedTask;
    protected virtual Task CleanupAsync() => Task.CompletedTask;
}
