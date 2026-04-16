using ArchQ.Application.Roles.DTOs;
using MediatR;

namespace ArchQ.Application.Roles.Commands.AddRole;

public class AddRoleCommand : IRequest<RoleChangeResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string ActorId { get; set; } = string.Empty;
}
