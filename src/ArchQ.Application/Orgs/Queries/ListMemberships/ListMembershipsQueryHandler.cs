using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Orgs.Queries.ListMemberships;

public class ListMembershipsQueryHandler : IRequestHandler<ListMembershipsQuery, ListMembershipsResponse>
{
    private readonly IGlobalUserRepository _globalUserRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly IUserRepository _userRepository;

    public ListMembershipsQueryHandler(
        IGlobalUserRepository globalUserRepository,
        ITenantRepository tenantRepository,
        IUserRepository userRepository)
    {
        _globalUserRepository = globalUserRepository;
        _tenantRepository = tenantRepository;
        _userRepository = userRepository;
    }

    public async Task<ListMembershipsResponse> Handle(ListMembershipsQuery request, CancellationToken cancellationToken)
    {
        var globalUser = await _globalUserRepository.GetByEmailAsync(request.Email);
        if (globalUser is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", "Global user not found.");
        }

        var memberships = new List<MembershipDto>();

        foreach (var m in globalUser.Memberships)
        {
            var tenant = await _tenantRepository.GetByIdAsync(m.TenantId);
            var user = await _userRepository.GetByIdAsync(m.UserId, m.TenantSlug);

            memberships.Add(new MembershipDto
            {
                TenantId = m.TenantId,
                TenantSlug = m.TenantSlug,
                DisplayName = tenant?.DisplayName ?? m.TenantSlug,
                Roles = user?.Roles ?? new List<string>(),
                IsActive = string.Equals(m.TenantSlug, request.CurrentTenantSlug, StringComparison.OrdinalIgnoreCase)
            });
        }

        return new ListMembershipsResponse { Memberships = memberships };
    }
}
