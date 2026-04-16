using ArchQ.Application.Roles.DTOs;
using ArchQ.Core.Authorization;
using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Roles.Commands.AddRole;

public class AddRoleCommandHandler : IRequestHandler<AddRoleCommand, RoleChangeResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;

    public AddRoleCommandHandler(IUserRepository userRepository, IAuditRepository auditRepository)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
    }

    public async Task<RoleChangeResponse> Handle(AddRoleCommand request, CancellationToken cancellationToken)
    {
        var roleLower = request.Role.ToLowerInvariant();

        if (!RolePolicy.IsValidRole(roleLower))
        {
            throw new DomainException("INVALID_ROLE", $"Role '{request.Role}' is not valid. Valid roles: {string.Join(", ", RolePolicy.ValidRoles)}.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantSlug)
            ?? throw new NotFoundException("USER_NOT_FOUND", $"User '{request.UserId}' not found in tenant '{request.TenantSlug}'.");

        // Idempotent: if user already has the role, return current state
        if (user.Roles.Contains(roleLower))
        {
            return new RoleChangeResponse
            {
                UserId = user.Id,
                Roles = user.Roles,
                UpdatedAt = user.UpdatedAt
            };
        }

        user.Roles.Add(roleLower);
        await _userRepository.UpdateAsync(user, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "ROLE_ASSIGNED",
            EntityType = "User",
            EntityId = request.UserId,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Role '{roleLower}' assigned to user '{request.UserId}'."
        });

        return new RoleChangeResponse
        {
            UserId = user.Id,
            Roles = user.Roles,
            UpdatedAt = user.UpdatedAt
        };
    }
}
