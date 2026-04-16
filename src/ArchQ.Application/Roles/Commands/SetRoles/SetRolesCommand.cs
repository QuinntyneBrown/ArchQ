using ArchQ.Application.Roles.DTOs;
using MediatR;

namespace ArchQ.Application.Roles.Commands.SetRoles;

public class SetRolesCommand : IRequest<RoleChangeResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public string ActorId { get; set; } = string.Empty;
}
