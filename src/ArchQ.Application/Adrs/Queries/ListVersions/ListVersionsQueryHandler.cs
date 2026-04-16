using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.ListVersions;

public class ListVersionsQueryHandler : IRequestHandler<ListVersionsQuery, ListVersionsResponse>
{
    private readonly IAdrVersionRepository _versionRepository;

    public ListVersionsQueryHandler(IAdrVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<ListVersionsResponse> Handle(ListVersionsQuery request, CancellationToken cancellationToken)
    {
        var versions = await _versionRepository.ListByAdrAsync(request.AdrId, request.TenantSlug);

        return new ListVersionsResponse
        {
            Items = versions.Select(v => new VersionItem
            {
                Id = v.Id,
                Version = v.Version,
                Title = v.Title,
                EditedBy = v.EditedBy,
                CreatedAt = v.CreatedAt
            }).ToList()
        };
    }
}
