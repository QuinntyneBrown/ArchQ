using MediatR;

namespace ArchQ.Application.Adrs.Queries.ListVersions;

public class ListVersionsQuery : IRequest<ListVersionsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}

public class ListVersionsResponse
{
    public List<VersionItem> Items { get; set; } = new();
}

public class VersionItem
{
    public string Id { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Title { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
