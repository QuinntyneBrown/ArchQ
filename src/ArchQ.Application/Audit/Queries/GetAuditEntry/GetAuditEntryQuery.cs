using MediatR;

namespace ArchQ.Application.Audit.Queries.GetAuditEntry;

public class GetAuditEntryQuery : IRequest<GetAuditEntryResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Id { get; set; } = string.Empty;
}

public class GetAuditEntryResponse
{
    public string Id { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public string EntityId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
    public string Details { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
