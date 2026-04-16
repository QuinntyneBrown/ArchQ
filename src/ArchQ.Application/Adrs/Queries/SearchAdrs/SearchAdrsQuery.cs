using MediatR;

namespace ArchQ.Application.Adrs.Queries.SearchAdrs;

public class SearchAdrsQuery : IRequest<SearchResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
    public string? Status { get; set; }
    public int PageSize { get; set; } = 20;
    public int Offset { get; set; } = 0;
}
