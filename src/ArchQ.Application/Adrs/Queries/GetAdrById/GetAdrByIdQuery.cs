using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetAdrById;

public class GetAdrByIdQuery : IRequest<AdrDetailResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}
