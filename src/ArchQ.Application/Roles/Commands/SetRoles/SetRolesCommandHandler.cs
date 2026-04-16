using ArchQ.Application.Roles.DTOs;
using ArchQ.Core.Authorization;
using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Roles.Commands.SetRoles;

public class SetRolesCommandHandler : IRequestHandler<SetRolesCommand, RoleChangeResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IAuditRepository _auditRepository;

    public SetRolesCommandHandler(IUserRepository userRepository, IAuditRepository auditRepository)
    {
        _userRepository = userRepository;
        _auditRepository = auditRepository;
    }

    public async Task<RoleChangeResponse> Handle(SetRolesCommand request, CancellationToken cancellationToken)
    {
        var normalizedRoles = request.Roles
            .Select(r => r.ToLowerInvariant())
            .Distinct()
            .ToList();

        foreach (var role in normalizedRoles)
        {
            if (!RolePolicy.IsValidRole(role))
            {
                throw new DomainException("INVALID_ROLE", $"Role '{role}' is not valid. Valid roles: {string.Join(", ", RolePolicy.ValidRoles)}.");
            }
        }

        var user = await _userRepository.GetByIdAsync(request.UserId, request.TenantSlug)
            ?? throw new NotFoundException("USER_NOT_FOUND", $"User '{request.UserId}' not found in tenant '{request.TenantSlug}'.");

        // Last-admin protection: if the user currently has admin but the new set does not
        if (user.Roles.Contains("admin") && !normalizedRoles.Contains("admin"))
        {
            var adminCount = await _userRepository.CountByRoleAsync("admin", request.TenantSlug);
            if (adminCount <= 1)
            {
                throw new ConflictException("LAST_ADMIN", "Cannot remove the last admin from this tenant.");
            }
        }

        var previousRoles = new List<string>(user.Roles);
        user.Roles = normalizedRoles;
        await _userRepository.UpdateAsync(user, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "ROLES_SET",
            EntityType = "User",
            EntityId = request.UserId,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Roles changed from [{string.Join(", ", previousRoles)}] to [{string.Join(", ", normalizedRoles)}] for user '{request.UserId}'."
        });

        return new RoleChangeResponse
        {
            UserId = user.Id,
            Roles = user.Roles,
            UpdatedAt = user.UpdatedAt
        };
    }
}
