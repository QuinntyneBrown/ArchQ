using ArchQ.Application.Roles.DTOs;
using MediatR;

namespace ArchQ.Application.Roles.Queries.GetUserRoles;

public class GetUserRolesQuery : IRequest<UserRolesResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
}
