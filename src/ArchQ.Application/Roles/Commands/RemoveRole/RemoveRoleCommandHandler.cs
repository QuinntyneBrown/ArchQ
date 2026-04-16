using ArchQ.Application.Roles.DTOs;
using ArchQ.Core.Authorization;
using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Roles.Commands.RemoveRole;

public class RemoveRoleCommandHandler : IRequestHandler<RemoveRoleCommand, RoleChangeResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;

    public RemoveRoleCommandHandler(IUserRepository userRepository, IAuditRepository auditRepository)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
    }

    public async Task<RoleChangeResponse> Handle(RemoveRoleCommand request, CancellationToken cancellationToken)
    {
        var roleLower = request.Role.ToLowerInvariant();

        if (!RolePolicy.IsValidRole(roleLower))
        {
            throw new DomainException("INVALID_ROLE", $"Role '{request.Role}' is not valid. Valid roles: {string.Join(", ", RolePolicy.ValidRoles)}.");
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantSlug)
            ?? throw new NotFoundException("USER_NOT_FOUND", $"User '{request.UserId}' not found in tenant '{request.TenantSlug}'.");

        // Last-admin protection
        if (roleLower == "admin" && user.Roles.Contains("admin"))
        {
            var adminCount = await _userRepository.CountByRoleAsync("admin", request.TenantSlug);
            if (adminCount <= 1)
            {
                throw new ConflictException("LAST_ADMIN", "Cannot remove the last admin from this tenant.");
            }
        }

        user.Roles.Remove(roleLower);
        await _userRepository.UpdateAsync(user, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "ROLE_REVOKED",
            EntityType = "User",
            EntityId = request.UserId,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Role '{roleLower}' removed from user '{request.UserId}'."
        });

        return new RoleChangeResponse
        {
            UserId = user.Id,
            Roles = user.Roles,
            UpdatedAt = user.UpdatedAt
        };
    }
}
