using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class GeneralNoteBenchmarks : CouchbaseBenchmarkBase
{
    private GeneralNoteRepository _repo = null!;
    private string _knownAdrId = string.Empty;
    private string _knownNoteId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new GeneralNoteRepository(Fixture.Context);
        _knownAdrId = $"adr::{Guid.NewGuid()}";

        for (int i = 0; i < 10; i++)
        {
            var note = new GeneralNote
            {
                Id = $"note::{Guid.NewGuid()}",
                AdrId = _knownAdrId,
                Title = $"Research Note {i + 1}",
                Body = $"Research findings for benchmark note {i + 1}. Contains analysis of different architectural approaches.",
                AuthorId = $"user::{Guid.NewGuid()}"
            };

            await _repo.CreateAsync(note, TenantSlug);
            if (i == 0) _knownNoteId = note.Id;
        }
    }

    [Benchmark(Description = "GeneralNote: Insert single document")]
    public async Task<GeneralNote> CreateNote()
    {
        var note = new GeneralNote
        {
            Id = $"note::{Guid.NewGuid()}",
            AdrId = _knownAdrId,
            Title = "Perf test note",
            Body = "Performance test general note body.",
            AuthorId = $"user::{Guid.NewGuid()}"
        };

        return await _repo.CreateAsync(note, TenantSlug);
    }

    [Benchmark(Description = "GeneralNote: Get by ID (KV lookup)")]
    public async Task<GeneralNote?> GetById()
    {
        return await _repo.GetByIdAsync(_knownNoteId, TenantSlug);
    }

    [Benchmark(Description = "GeneralNote: List by ADR (N1QL)")]
    public async Task<List<GeneralNote>> ListByAdr()
    {
        return await _repo.ListByAdrAsync(_knownAdrId, TenantSlug);
    }

    [Benchmark(Description = "GeneralNote: Update with CAS")]
    public async Task<GeneralNote> UpdateNote()
    {
        var note = (await _repo.GetByIdAsync(_knownNoteId, TenantSlug))!;
        note.Body = $"Updated at {DateTime.UtcNow:O}";
        note.UpdatedAt = DateTime.UtcNow;
        return await _repo.UpdateAsync(note, TenantSlug);
    }
}
