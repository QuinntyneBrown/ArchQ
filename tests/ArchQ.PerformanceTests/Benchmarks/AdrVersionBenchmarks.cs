using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AdrVersionBenchmarks : CouchbaseBenchmarkBase
{
    private AdrVersionRepository _repo = null!;
    private string _knownAdrId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new AdrVersionRepository(Fixture.Context);
        _knownAdrId = $"adr::{Guid.NewGuid()}";

        for (int i = 1; i <= 10; i++)
        {
            var version = new AdrVersion
            {
                Id = $"adr_version::{Guid.NewGuid()}",
                AdrId = _knownAdrId,
                Version = i,
                Title = $"ADR Title v{i}",
                Body = $"This is version {i} of the ADR. Contains detailed analysis of the architectural decision and its trade-offs at version {i}.",
                EditedBy = $"user::{Guid.NewGuid()}"
            };

            await _repo.CreateAsync(version, TenantSlug);
        }
    }

    [Benchmark(Description = "AdrVersion: Insert version snapshot")]
    public async Task<AdrVersion> CreateVersion()
    {
        var version = new AdrVersion
        {
            Id = $"adr_version::{Guid.NewGuid()}",
            AdrId = _knownAdrId,
            Version = Random.Shared.Next(100, 999),
            Title = "Perf test version snapshot",
            Body = "Performance test version body content.",
            EditedBy = $"user::{Guid.NewGuid()}"
        };

        return await _repo.CreateAsync(version, TenantSlug);
    }

    [Benchmark(Description = "AdrVersion: List by ADR (N1QL)")]
    public async Task<List<AdrVersion>> ListByAdr()
    {
        return await _repo.ListByAdrAsync(_knownAdrId, TenantSlug);
    }

    [Benchmark(Description = "AdrVersion: Get specific version (N1QL)")]
    public async Task<AdrVersion?> GetByVersion()
    {
        return await _repo.GetByVersionAsync(_knownAdrId, 5, TenantSlug);
    }
}
