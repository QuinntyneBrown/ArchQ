using MediatR;

namespace ArchQ.Application.Adrs.Queries.CompareVersions;

public class CompareVersionsQuery : IRequest<CompareVersionsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public int FromVersion { get; set; }
    public int ToVersion { get; set; }
}

public class CompareVersionsResponse
{
    public int FromVersion { get; set; }
    public int ToVersion { get; set; }
    public List<DiffLine> TitleDiff { get; set; } = new();
    public List<DiffLine> BodyDiff { get; set; } = new();
}

public class DiffLine
{
    public string Type { get; set; } = string.Empty; // "added", "removed", "unchanged"
    public string Content { get; set; } = string.Empty;
}
