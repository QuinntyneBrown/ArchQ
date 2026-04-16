using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AuditBenchmarks : CouchbaseBenchmarkBase
{
    private AuditRepository _repo = null!;
    private string _knownResourceId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new AuditRepository(Fixture.Context);
        _knownResourceId = $"adr::{Guid.NewGuid()}";

        var actions = new[] { "adr.created", "adr.updated", "adr.status_changed", "comment.added", "approval.submitted" };

        for (int i = 0; i < 30; i++)
        {
            var entry = new AuditEntry
            {
                Id = Guid.NewGuid().ToString(),
                TenantId = TenantSlug,
                Action = actions[i % actions.Length],
                EntityType = "adr",
                EntityId = i < 10 ? _knownResourceId : $"adr::{Guid.NewGuid()}",
                UserId = $"user::{Guid.NewGuid()}",
                Timestamp = DateTime.UtcNow.AddMinutes(-i),
                Details = $"Audit entry {i} — benchmark operation"
            };

            await _repo.WriteEntryAsync(entry);
        }
    }

    [Benchmark(Description = "Audit: Write single entry")]
    public async Task WriteAuditEntry()
    {
        var entry = new AuditEntry
        {
            Id = Guid.NewGuid().ToString(),
            TenantId = TenantSlug,
            Action = "adr.updated",
            EntityType = "adr",
            EntityId = _knownResourceId,
            UserId = $"user::{Guid.NewGuid()}",
            Timestamp = DateTime.UtcNow,
            Details = "Perf test audit write"
        };

        await _repo.WriteEntryAsync(entry);
    }

    [Benchmark(Description = "Audit: List with no filters")]
    public async Task<AuditListResult> ListAuditNoFilter()
    {
        return await _repo.ListAsync(TenantSlug, new AuditFilterParams(), 25, 0);
    }

    [Benchmark(Description = "Audit: List filtered by action")]
    public async Task<AuditListResult> ListAuditByAction()
    {
        var filters = new AuditFilterParams { Action = "adr.created" };
        return await _repo.ListAsync(TenantSlug, filters, 25, 0);
    }

    [Benchmark(Description = "Audit: List by resource ID")]
    public async Task<List<AuditEntry>> ListByResource()
    {
        return await _repo.ListByResourceAsync(_knownResourceId, TenantSlug);
    }
}
