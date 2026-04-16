using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.Infrastructure.Search;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class SearchBenchmarks : CouchbaseBenchmarkBase
{
    private N1qlSearchService _searchService = null!;
    private AdrRepository _adrRepo = null!;

    [Params(20, 100)]
    public int DocumentCount { get; set; }

    protected override async Task SetupAsync()
    {
        _searchService = new N1qlSearchService(Fixture.Context);
        _adrRepo = new AdrRepository(Fixture.Context);

        var topics = new[] { "microservices", "event-driven architecture", "CQRS pattern", "database sharding", "API gateway", "service mesh", "caching strategy", "message queue", "authentication flow", "deployment pipeline" };
        var bodies = new[]
        {
            "We propose adopting a microservices architecture to improve scalability and team autonomy.",
            "The event-driven approach using message queues will decouple our services and improve resilience.",
            "Implementing CQRS will separate read and write models, optimizing each for their specific workload.",
            "Database sharding across geographic regions will reduce latency for our global user base.",
            "An API gateway will centralize cross-cutting concerns like rate limiting and authentication.",
            "The service mesh provides observability, traffic management, and security between services.",
            "A multi-layer caching strategy with Redis and CDN will reduce database load by 80%.",
            "RabbitMQ message queues will handle async processing and decouple our payment pipeline.",
            "OAuth 2.0 with PKCE flow provides secure authentication for our single-page application.",
            "Blue-green deployment pipeline with automated rollback reduces deployment risk."
        };

        for (int i = 0; i < DocumentCount; i++)
        {
            var idx = i % topics.Length;
            var adr = new Adr
            {
                Id = $"adr::{Guid.NewGuid()}",
                AdrNumber = $"ADR-{(i + 1):D3}",
                Title = $"{topics[idx]} — Decision #{i + 1}",
                Body = $"{bodies[idx]} Additional context for document {i + 1}: {string.Join(" ", Enumerable.Range(0, 20).Select(n => $"word{n}"))}",
                Status = i % 3 == 0 ? "draft" : i % 3 == 1 ? "in_review" : "approved",
                AuthorId = $"user::{Guid.NewGuid()}",
                Tags = new List<string> { topics[idx].Split(' ')[0].ToLowerInvariant(), "benchmark" }
            };

            await _adrRepo.CreateAsync(adr, TenantSlug);
        }
    }

    [Benchmark(Description = "Search: Single keyword")]
    public async Task<SearchResult> SearchSingleKeyword()
    {
        return await _searchService.SearchAsync("microservices", TenantSlug, null, 25, 0);
    }

    [Benchmark(Description = "Search: Multi-word query")]
    public async Task<SearchResult> SearchMultiWord()
    {
        return await _searchService.SearchAsync("event driven", TenantSlug, null, 25, 0);
    }

    [Benchmark(Description = "Search: With status filter")]
    public async Task<SearchResult> SearchWithStatusFilter()
    {
        return await _searchService.SearchAsync("architecture", TenantSlug, "approved", 25, 0);
    }

    [Benchmark(Description = "Search: Broad query (many matches)")]
    public async Task<SearchResult> SearchBroadQuery()
    {
        return await _searchService.SearchAsync("decision", TenantSlug, null, 25, 0);
    }

    [Benchmark(Description = "Search: Narrow query (few matches)")]
    public async Task<SearchResult> SearchNarrowQuery()
    {
        return await _searchService.SearchAsync("PKCE", TenantSlug, null, 25, 0);
    }
}
