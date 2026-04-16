using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.SearchAdrs;

public class SearchAdrsQueryHandler : IRequestHandler<SearchAdrsQuery, SearchResponse>
{
    private readonly ISearchService _searchService;

    public SearchAdrsQueryHandler(ISearchService searchService)
    {
        _searchService = searchService;
    }

    public async Task<SearchResponse> Handle(SearchAdrsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 50);
        var offset = Math.Max(request.Offset, 0);

        var result = await _searchService.SearchAsync(
            request.Query,
            request.TenantSlug,
            request.Status,
            pageSize,
            offset);

        return new SearchResponse
        {
            Results = result.Results.Select(r => new SearchResultItemResponse
            {
                Id = r.Id,
                AdrNumber = r.AdrNumber,
                Title = r.Title,
                Status = r.Status,
                Score = r.Score,
                Snippet = r.Snippet
            }).ToList(),
            TotalHits = result.TotalHits,
            Took = result.Took
        };
    }
}
