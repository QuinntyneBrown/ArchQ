using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using ArchQ.Application.Common;
using ArchQ.Application.Auth.Commands.Login;
using MediatR;

namespace ArchQ.Application.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, RefreshTokenResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;
    private readonly IUserRepository _userRepository;
    private readonly ITenantRepository _tenantRepository;

    public RefreshTokenCommandHandler(
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository,
        IUserRepository userRepository,
        ITenantRepository tenantRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
        _userRepository = userRepository;
        _tenantRepository = tenantRepository;
    }

    public async Task<RefreshTokenResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        // Verify the refresh JWT
        var payload = _tokenService.VerifyRefreshToken(request.RefreshToken);
        if (payload is null)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
        }

        // Hash the token ID and look up in repository
        var tokenHash = TokenHasher.ComputeSha256(payload.TokenId);
        var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

        if (storedToken is null || storedToken.Revoked)
        {
            // Potential token reuse — revoke entire family
            if (storedToken is not null)
            {
                await _refreshTokenRepository.RevokeAllInFamilyAsync(storedToken.Family);
            }

            throw new DomainException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
        }

        if (storedToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
        }

        // Get user
        var user = await _userRepository.GetByIdAsync(storedToken.UserId, storedToken.TenantSlug);
        if (user is null)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
        }

        // Get tenant
        var tenant = await _tenantRepository.GetBySlugAsync(storedToken.TenantSlug);
        if (tenant is null)
        {
            throw new DomainException("INVALID_REFRESH_TOKEN", "Invalid or expired refresh token.");
        }

        // Create new tokens (same family)
        var newAccessToken = _tokenService.CreateAccessToken(user, tenant.Id, tenant.Slug, user.Roles);
        var newRefreshTokenJwt = _tokenService.CreateRefreshToken(user.Id, storedToken.Family);

        // Mark old token as revoked
        var newPayload = _tokenService.VerifyRefreshToken(newRefreshTokenJwt)!;
        var newTokenHash = TokenHasher.ComputeSha256(newPayload.TokenId);

        storedToken.Revoked = true;
        storedToken.RevokedAt = DateTime.UtcNow;
        storedToken.ReplacedBy = newTokenHash;
        await _refreshTokenRepository.UpdateAsync(storedToken);

        // Store new refresh token
        var newStoredToken = new Core.Entities.RefreshToken
        {
            TokenHash = newTokenHash,
            Family = storedToken.Family,
            UserId = storedToken.UserId,
            TenantSlug = storedToken.TenantSlug,
            IssuedAt = DateTime.UtcNow,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            Revoked = false
        };

        await _refreshTokenRepository.CreateAsync(newStoredToken);

        return new RefreshTokenResponse
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshTokenJwt,
            User = new LoginUserDto
            {
                Id = user.Id,
                Email = user.Email,
                FullName = user.FullName,
                Roles = user.Roles
            },
            Tenant = new LoginTenantDto
            {
                Id = tenant.Id,
                Slug = tenant.Slug,
                DisplayName = tenant.DisplayName
            },
            Memberships = new List<LoginMembershipDto>()
        };
    }
}
