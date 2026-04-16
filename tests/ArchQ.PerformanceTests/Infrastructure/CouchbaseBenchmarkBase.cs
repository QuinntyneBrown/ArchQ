using BenchmarkDotNet.Attributes;

namespace ArchQ.PerformanceTests.Infrastructure;

/// <summary>
/// Base class for all Couchbase benchmarks. The scope is already provisioned
/// by Program.cs before BenchmarkDotNet launches child processes.
/// Each benchmark just connects and seeds its own test data.
/// </summary>
public abstract class CouchbaseBenchmarkBase
{
    protected CouchbaseFixture Fixture => CouchbaseFixture.Shared;
    protected string TenantSlug => Fixture.TenantSlug;

    [GlobalSetup]
    public async Task GlobalSetup()
    {
        // Verify connectivity — the scope is already provisioned by Program.cs
        await Fixture.Context.GetBucketAsync();
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
