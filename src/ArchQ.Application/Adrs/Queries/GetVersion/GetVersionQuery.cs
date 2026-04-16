using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetVersion;

public class GetVersionQuery : IRequest<GetVersionResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public int Version { get; set; }
}

public class GetVersionResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
