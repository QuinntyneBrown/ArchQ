using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using ArchQ.Application.Common;
using MediatR;

namespace ArchQ.Application.Orgs.Commands.SwitchOrg;

public class SwitchOrgCommandHandler : IRequestHandler<SwitchOrgCommand, SwitchOrgResponse>
{
    private readonly IGlobalUserRepository _globalUserRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IAuditRepository _auditRepository;

    public SwitchOrgCommandHandler(
        IGlobalUserRepository globalUserRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository,
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IAuditRepository auditRepository)
    {
        _globalUserRepository = globalUserRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _auditRepository = auditRepository;
    }

    public async Task<SwitchOrgResponse> Handle(SwitchOrgCommand request, CancellationToken cancellationToken)
    {
        var emailLower = request.Email.Trim().ToLowerInvariant();

        // Look up global user
        var globalUser = await _globalUserRepository.GetByEmailAsync(emailLower);
        if (globalUser is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", "Global user not found.");
        }

        // Find membership for target tenant
        var membership = globalUser.Memberships
            .FirstOrDefault(m => string.Equals(m.TenantSlug, request.TenantSlug, StringComparison.OrdinalIgnoreCase));

        if (membership is null || !string.Equals(membership.Status, "active", StringComparison.OrdinalIgnoreCase))
        {
            throw new ForbiddenException("NOT_A_MEMBER", "You are not a member of this organization.");
        }

        // Get tenant
        var tenant = await _tenantRepository.GetBySlugAsync(membership.TenantSlug);
        if (tenant is null)
        {
            throw new NotFoundException("TENANT_NOT_FOUND", "Organization not found.");
        }

        // Get user in target tenant for roles
        var user = await _userRepository.GetByIdAsync(membership.UserId, membership.TenantSlug);
        if (user is null)
        {
            throw new NotFoundException("USER_NOT_FOUND", "User not found in target organization.");
        }

        // Revoke current refresh token family
        if (!string.IsNullOrEmpty(request.CurrentRefreshToken))
        {
            var currentPayload = _tokenService.VerifyRefreshToken(request.CurrentRefreshToken);
            if (currentPayload is not null)
            {
                await _refreshTokenRepository.RevokeAllInFamilyAsync(currentPayload.Family);
            }
        }

        // Issue new tokens
        var accessToken = _tokenService.CreateAccessToken(user, tenant.Id, tenant.Slug, user.Roles);
        var family = Guid.NewGuid().ToString();
        var refreshTokenJwt = _tokenService.CreateRefreshToken(user.Id, family);

        // Store new refresh token
        var refreshTokenPayload = _tokenService.VerifyRefreshToken(refreshTokenJwt);
        var tokenHash = TokenHasher.ComputeSha256(refreshTokenPayload!.TokenId);

        var refreshToken = new RefreshToken
        {
            TokenHash = tokenHash,
            Family = family,
            UserId = user.Id,
            TenantSlug = membership.TenantSlug,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _refreshTokenRepository.CreateAsync(refreshToken);

        // Audit
        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = tenant.Id,
            Action = "ORG_SWITCHED",
            EntityType = "User",
            EntityId = user.Id,
            UserId = user.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"User switched to organization '{tenant.DisplayName}' ({tenant.Slug}).",
            IpAddress = request.IpAddress,
            UserAgent = request.UserAgent
        });

        return new SwitchOrgResponse
        {
            Tenant = new SwitchOrgTenantDto
            {
                Id = tenant.Id,
                Slug = tenant.Slug,
                DisplayName = tenant.DisplayName
            },
            User = new SwitchOrgUserDto
            {
                Id = user.Id,
                Roles = user.Roles
            },
            AccessToken = accessToken,
            RefreshToken = refreshTokenJwt
        };
    }
}
