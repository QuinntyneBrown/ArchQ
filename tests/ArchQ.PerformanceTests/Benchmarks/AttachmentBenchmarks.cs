using BenchmarkDotNet.Attributes;
using ArchQ.Core.Entities;
using ArchQ.Infrastructure.Persistence.Repositories;
using ArchQ.PerformanceTests.Infrastructure;

namespace ArchQ.PerformanceTests.Benchmarks;

[MemoryDiagnoser]
[SimpleJob(warmupCount: 3, iterationCount: 10)]
public class AttachmentBenchmarks : CouchbaseBenchmarkBase
{
    private AttachmentRepository _repo = null!;
    private string _knownAdrId = string.Empty;
    private string _knownAttachmentId = string.Empty;

    protected override async Task SetupAsync()
    {
        _repo = new AttachmentRepository(Fixture.Context);
        _knownAdrId = $"adr::{Guid.NewGuid()}";

        for (int i = 0; i < 10; i++)
        {
            var attachment = new AttachmentMeta
            {
                Id = $"attach::{Guid.NewGuid()}",
                AdrId = _knownAdrId,
                FileName = $"diagram-{i}.png",
                DisplayName = $"Architecture Diagram {i}",
                Description = $"C4 diagram showing component {i} interactions",
                ContentType = "image/png",
                FileSize = Random.Shared.Next(10_000, 5_000_000),
                StorageKey = $"tenants/{TenantSlug}/attachments/{Guid.NewGuid()}.png",
                AuthorId = $"user::{Guid.NewGuid()}"
            };

            await _repo.CreateAsync(attachment, TenantSlug);
            if (i == 0) _knownAttachmentId = attachment.Id;
        }
    }

    [Benchmark(Description = "Attachment: Insert metadata")]
    public async Task<AttachmentMeta> CreateAttachment()
    {
        var attachment = new AttachmentMeta
        {
            Id = $"attach::{Guid.NewGuid()}",
            AdrId = _knownAdrId,
            FileName = "perf-test.pdf",
            DisplayName = "Perf Test Document",
            Description = "Performance test attachment",
            ContentType = "application/pdf",
            FileSize = 12345,
            StorageKey = $"tenants/{TenantSlug}/attachments/{Guid.NewGuid()}.pdf",
            AuthorId = $"user::{Guid.NewGuid()}"
        };

        return await _repo.CreateAsync(attachment, TenantSlug);
    }

    [Benchmark(Description = "Attachment: Get by ID (KV lookup)")]
    public async Task<AttachmentMeta?> GetById()
    {
        return await _repo.GetByIdAsync(_knownAttachmentId, TenantSlug);
    }

    [Benchmark(Description = "Attachment: List by ADR (N1QL)")]
    public async Task<List<AttachmentMeta>> ListByAdr()
    {
        return await _repo.ListByAdrAsync(_knownAdrId, TenantSlug);
    }
}
