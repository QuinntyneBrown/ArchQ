using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class MeetingNoteBenchmarks : CouchbaseBenchmarkBase
{
    private MeetingNoteRepository _repo = null!;
    private string _knownAdrId = string.Empty;
    private string _knownNoteId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new MeetingNoteRepository(Fixture.Context);
        _knownAdrId = $"adr::{Guid.NewGuid()}";

        for (int i = 0; i < 10; i++)
        {
            var note = new MeetingNote
            {
                Id = $"mnote::{Guid.NewGuid()}",
                AdrId = _knownAdrId,
                Title = $"Sprint Planning Meeting {i + 1}",
                MeetingDate = DateTime.UtcNow.AddDays(-i),
                Attendees = new List<string> { "user-1", "user-2", "user-3" },
                Body = $"Discussion notes for meeting {i + 1}. Covered architecture decision trade-offs and implementation timeline.",
                AuthorId = $"user::{Guid.NewGuid()}"
            };

            await _repo.CreateAsync(note, TenantSlug);
            if (i == 0) _knownNoteId = note.Id;
        }
    }

    [Benchmark(Description = "MeetingNote: Insert single document")]
    public async Task<MeetingNote> CreateMeetingNote()
    {
        var note = new MeetingNote
        {
            Id = $"mnote::{Guid.NewGuid()}",
            AdrId = _knownAdrId,
            Title = "Perf test meeting note",
            MeetingDate = DateTime.UtcNow,
            Attendees = new List<string> { "user-1", "user-2" },
            Body = "Performance test meeting note body content.",
            AuthorId = $"user::{Guid.NewGuid()}"
        };

        return await _repo.CreateAsync(note, TenantSlug);
    }

    [Benchmark(Description = "MeetingNote: Get by ID (KV lookup)")]
    public async Task<MeetingNote?> GetById()
    {
        return await _repo.GetByIdAsync(_knownNoteId, TenantSlug);
    }

    [Benchmark(Description = "MeetingNote: List by ADR (N1QL)")]
    public async Task<List<MeetingNote>> ListByAdr()
    {
        return await _repo.ListByAdrAsync(_knownAdrId, TenantSlug);
    }

    [Benchmark(Description = "MeetingNote: Update with CAS")]
    public async Task<MeetingNote> UpdateNote()
    {
        var note = (await _repo.GetByIdAsync(_knownNoteId, TenantSlug))!;
        note.Body = $"Updated at {DateTime.UtcNow:O}";
        note.UpdatedAt = DateTime.UtcNow;
        return await _repo.UpdateAsync(note, TenantSlug);
    }
}
