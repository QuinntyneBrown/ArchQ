using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAuditRepository
{
    Task WriteEntryAsync(AuditEntry entry);
    Task<AuditListResult> ListAsync(string tenantSlug, AuditFilterParams filters, int pageSize, int offset);
    Task<AuditEntry?> GetByIdAsync(string id, string tenantSlug);
    Task<List<AuditEntry>> ListByResourceAsync(string resourceId, string tenantSlug);
}

public class AuditFilterParams
{
    public string? Action { get; set; }
    public string? ResourceId { get; set; }
    public string? ActorId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
}

public class AuditListResult
{
    public List<AuditEntry> Items { get; set; } = new();
    public int TotalCount { get; set; }
}
