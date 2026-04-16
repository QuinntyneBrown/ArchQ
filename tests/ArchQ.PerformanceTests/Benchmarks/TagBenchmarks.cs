using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class TagBenchmarks : CouchbaseBenchmarkBase
{
    private TagRepository _repo = null!;
    private string _knownTagSlug = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new TagRepository(Fixture.Context);

        var tagNames = new[] { "architecture", "security", "performance", "api-design", "database", "caching", "auth", "testing", "deployment", "monitoring" };

        foreach (var name in tagNames)
        {
            var slug = name.ToLowerInvariant();
            var tag = new Tag
            {
                Id = Tag.GenerateId(slug),
                Name = name,
                Slug = slug,
                UsageCount = Random.Shared.Next(1, 50),
                TenantSlug = TenantSlug
            };

            await _repo.CreateAsync(tag, TenantSlug);
        }

        _knownTagSlug = "architecture";
    }

    [Benchmark(Description = "Tag: Insert single document")]
    public async Task<Tag> CreateTag()
    {
        var slug = $"bench-{Guid.NewGuid():N}"[..20];
        var tag = new Tag
        {
            Id = Tag.GenerateId(slug),
            Name = slug,
            Slug = slug,
            UsageCount = 0,
            TenantSlug = TenantSlug
        };

        return await _repo.CreateAsync(tag, TenantSlug);
    }

    [Benchmark(Description = "Tag: Get by slug (KV lookup)")]
    public async Task<Tag?> GetTagBySlug()
    {
        return await _repo.GetBySlugAsync(_knownTagSlug, TenantSlug);
    }

    [Benchmark(Description = "Tag: List all (N1QL)")]
    public async Task<List<Tag>> ListAllTags()
    {
        return await _repo.ListAllAsync(TenantSlug);
    }

    [Benchmark(Description = "Tag: Search by prefix (N1QL LIKE)")]
    public async Task<List<Tag>> SearchTags()
    {
        return await _repo.SearchAsync("arch", TenantSlug);
    }

    [Benchmark(Description = "Tag: Increment usage count")]
    public async Task IncrementUsage()
    {
        await _repo.IncrementUsageAsync(_knownTagSlug, TenantSlug);
    }
}
