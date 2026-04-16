using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AdrBenchmarks : CouchbaseBenchmarkBase
{
    private AdrRepository _repo = null!;
    private readonly List<string> _seededAdrIds = new();
    private string _knownAdrId = string.Empty;

    [Params(10, 50)]
    public int SeedCount { get; set; }

    protected override async Task SetupAsync()
    {
        _repo = new AdrRepository(Fixture.Context);

        for (int i = 0; i < SeedCount; i++)
        {
            var adr = new Adr
            {
                Id = $"adr::{Guid.NewGuid()}",
                AdrNumber = $"ADR-{(i + 1):D3}",
                Title = $"Benchmark ADR {i + 1} — Architecture Decision on Component {i}",
                Body = $"This is the body of benchmark ADR {i + 1}. It contains details about the decision, context, and consequences of choosing approach {i} for the system architecture.",
                Status = i % 3 == 0 ? "draft" : i % 3 == 1 ? "in_review" : "approved",
                AuthorId = $"user::{Guid.NewGuid()}",
                Tags = new List<string> { "benchmark", $"tag-{i % 5}" },
                Version = 1
            };

            await _repo.CreateAsync(adr, TenantSlug);
            _seededAdrIds.Add(adr.Id);
        }

        _knownAdrId = _seededAdrIds[0];
    }

    [Benchmark(Description = "ADR: Insert single document")]
    public async Task<Adr> CreateAdr()
    {
        var adr = new Adr
        {
            Id = $"adr::{Guid.NewGuid()}",
            AdrNumber = $"ADR-{Random.Shared.Next(1000, 9999):D4}",
            Title = "Perf test ADR — dynamic insert",
            Body = "Body content for performance testing. Evaluating insert latency against a real Couchbase cluster.",
            Status = "draft",
            AuthorId = $"user::{Guid.NewGuid()}",
            Tags = new List<string> { "perf", "test" }
        };

        return await _repo.CreateAsync(adr, TenantSlug);
    }

    [Benchmark(Description = "ADR: Get by ID (KV lookup)")]
    public async Task<Adr?> GetAdrById()
    {
        return await _repo.GetByIdAsync(_knownAdrId, TenantSlug);
    }

    [Benchmark(Description = "ADR: Update with CAS")]
    public async Task<Adr> UpdateAdr()
    {
        var adr = (await _repo.GetByIdAsync(_knownAdrId, TenantSlug))!;
        adr.Title = $"Updated at {DateTime.UtcNow:O}";
        adr.UpdatedAt = DateTime.UtcNow;
        return await _repo.UpdateAsync(adr, TenantSlug);
    }

    [Benchmark(Description = "ADR: List with N1QL (no filters)")]
    public async Task<AdrListResult> ListAdrsNoFilter()
    {
        var listParams = new AdrListParams { PageSize = 25 };
        return await _repo.ListAsync(listParams, TenantSlug);
    }

    [Benchmark(Description = "ADR: List filtered by status")]
    public async Task<AdrListResult> ListAdrsFilteredByStatus()
    {
        var listParams = new AdrListParams { Status = "draft", PageSize = 25 };
        return await _repo.ListAsync(listParams, TenantSlug);
    }

    [Benchmark(Description = "ADR: List filtered by tags")]
    public async Task<AdrListResult> ListAdrsFilteredByTags()
    {
        var listParams = new AdrListParams { Tags = new List<string> { "benchmark" }, PageSize = 25 };
        return await _repo.ListAsync(listParams, TenantSlug);
    }

    [Benchmark(Description = "ADR: Count with filters")]
    public async Task<int> CountAdrsFiltered()
    {
        var listParams = new AdrListParams { Status = "draft" };
        return await _repo.CountAsync(listParams, TenantSlug);
    }

    [Benchmark(Description = "ADR: Get max ADR number")]
    public async Task<int> GetMaxAdrNumber()
    {
        return await _repo.GetMaxAdrNumberAsync(TenantSlug);
    }
}
