using MediatR;

namespace ArchQ.Application.Audit.Queries.ListAuditEntries;

public class ListAuditEntriesQuery : IRequest<ListAuditEntriesResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string? Action { get; set; }
    public string? ResourceId { get; set; }
    public string? ActorId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int PageSize { get; set; } = 25;
    public int Offset { get; set; } = 0;
}

public class ListAuditEntriesResponse
{
    public List<AuditEntryItem> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageSize { get; set; }
    public int Offset { get; set; }
}

public class AuditEntryItem
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
}
