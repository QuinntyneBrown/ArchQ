using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class CommentBenchmarks : CouchbaseBenchmarkBase
{
    private CommentRepository _repo = null!;
    private string _knownAdrId = string.Empty;
    private string _knownCommentId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new CommentRepository(Fixture.Context);
        _knownAdrId = $"adr::{Guid.NewGuid()}";

        for (int i = 0; i < 15; i++)
        {
            var comment = new Comment
            {
                Id = $"comment::{Guid.NewGuid()}",
                AdrId = _knownAdrId,
                ParentCommentId = i > 5 ? _knownCommentId : null,
                Body = $"Benchmark comment {i}: This is a detailed discussion about the architecture decision. We should consider the trade-offs.",
                AuthorId = $"user::{Guid.NewGuid()}"
            };

            await _repo.CreateAsync(comment, TenantSlug);

            if (i == 0) _knownCommentId = comment.Id;
        }
    }

    [Benchmark(Description = "Comment: Insert single document")]
    public async Task<Comment> CreateComment()
    {
        var comment = new Comment
        {
            Id = $"comment::{Guid.NewGuid()}",
            AdrId = _knownAdrId,
            Body = "Performance test comment — measuring insert latency.",
            AuthorId = $"user::{Guid.NewGuid()}"
        };

        return await _repo.CreateAsync(comment, TenantSlug);
    }

    [Benchmark(Description = "Comment: Get by ID (KV lookup)")]
    public async Task<Comment?> GetCommentById()
    {
        return await _repo.GetByIdAsync(_knownCommentId, TenantSlug);
    }

    [Benchmark(Description = "Comment: List by ADR (N1QL)")]
    public async Task<List<Comment>> ListByAdr()
    {
        return await _repo.ListByAdrAsync(_knownAdrId, TenantSlug);
    }

    [Benchmark(Description = "Comment: Update with CAS")]
    public async Task<Comment> UpdateComment()
    {
        var comment = (await _repo.GetByIdAsync(_knownCommentId, TenantSlug))!;
        comment.Body = $"Updated at {DateTime.UtcNow:O}";
        comment.Edited = true;
        comment.UpdatedAt = DateTime.UtcNow;
        return await _repo.UpdateAsync(comment, TenantSlug);
    }
}
