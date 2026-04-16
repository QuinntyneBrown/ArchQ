using ArchQ.Application.Roles.DTOs;
using ArchQ.Core.Authorization;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Roles.Queries.GetUserRoles;

public class GetUserRolesQueryHandler : IRequestHandler<GetUserRolesQuery, UserRolesResponse>
{
    private readonly IUserRepository _userRepository;

    public GetUserRolesQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<UserRolesResponse> Handle(GetUserRolesQuery request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantSlug)
            ?? throw new NotFoundException("USER_NOT_FOUND", $"User '{request.UserId}' not found in tenant '{request.TenantSlug}'.");

        return new UserRolesResponse
        {
            UserId = user.Id,
            Roles = user.Roles,
            Permissions = RolePolicy.GetEffectivePermissions(user.Roles)
        };
    }
}
