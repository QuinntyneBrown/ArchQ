using MediatR;

namespace ArchQ.Application.Orgs.Queries.ListMemberships;

public class ListMembershipsQuery : IRequest<ListMembershipsResponse>
{
    public string Email { get; set; } = string.Empty;
    public string CurrentTenantSlug { get; set; } = string.Empty;
}
