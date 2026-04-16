using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetVersion;

public class GetVersionQueryHandler : IRequestHandler<GetVersionQuery, GetVersionResponse>
{
    private readonly IAdrVersionRepository _versionRepository;

    public GetVersionQueryHandler(IAdrVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<GetVersionResponse> Handle(GetVersionQuery request, CancellationToken cancellationToken)
    {
        var version = await _versionRepository.GetByVersionAsync(request.AdrId, request.Version, request.TenantSlug)
            ?? throw new NotFoundException("VERSION_NOT_FOUND", $"Version {request.Version} not found for this ADR.");

        return new GetVersionResponse
        {
            Id = version.Id,
            AdrId = version.AdrId,
            Version = version.Version,
            Title = version.Title,
            Body = version.Body,
            EditedBy = version.EditedBy,
            CreatedAt = version.CreatedAt
        };
    }
}
