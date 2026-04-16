using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetAdrById;

public class GetAdrByIdQueryHandler : IRequestHandler<GetAdrByIdQuery, AdrDetailResponse>
{
    private readonly IAdrRepository _adrRepository;

    public GetAdrByIdQueryHandler(IAdrRepository adrRepository)
    {
        _adrRepository = adrRepository;
    }

    public async Task<AdrDetailResponse> Handle(GetAdrByIdQuery request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new NotFoundException("ADR_NOT_FOUND", $"ADR '{request.AdrId}' was not found.");

        return AdrDetailResponse.FromEntity(adr);
    }
}
