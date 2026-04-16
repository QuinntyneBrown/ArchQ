using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace ArchQ.Infrastructure.Identity;

// TODO: Migrate from HS256 to RS256 for token signing. RS256 allows public-key verification
// without sharing the signing secret, which is preferred for distributed / multi-service deployments.
public class TokenService : ITokenService
{
    private readonly string _secret;
    private readonly string _issuer;
    private readonly string _audience;
    private readonly SymmetricSecurityKey _signingKey;

    public TokenService(IConfiguration configuration)
    {
        _secret = configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is not configured.");
        _issuer = configuration["Jwt:Issuer"] ?? "archq";
        _audience = configuration["Jwt:Audience"] ?? "archq";
        _signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
    }

    public string CreateAccessToken(User user, string tenantId, string tenantSlug, List<string> roles)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new("tenant_id", tenantId),
            new("tenant_slug", tenantSlug),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        foreach (var role in roles)
        {
            claims.Add(new Claim("roles", role));
        }

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string CreateRefreshToken(string userId, string family)
    {
        var tokenId = Guid.NewGuid().ToString();

        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, userId),
            new(JwtRegisteredClaimNames.Jti, tokenId),
            new("family", family)
        };

        var credentials = new SigningCredentials(_signingKey, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.AddDays(7),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public AccessTokenPayload? VerifyAccessToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, GetValidationParameters(), out _);

            return new AccessTokenPayload
            {
                UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty,
                Email = principal.FindFirstValue(JwtRegisteredClaimNames.Email) ?? string.Empty,
                TenantId = principal.FindFirstValue("tenant_id") ?? string.Empty,
                TenantSlug = principal.FindFirstValue("tenant_slug") ?? string.Empty,
                Roles = principal.FindAll("roles").Select(c => c.Value).ToList()
            };
        }
        catch
        {
            return null;
        }
    }

    public RefreshTokenPayload? VerifyRefreshToken(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler { MapInboundClaims = false };
            var principal = handler.ValidateToken(token, GetValidationParameters(), out _);

            return new RefreshTokenPayload
            {
                UserId = principal.FindFirstValue(JwtRegisteredClaimNames.Sub) ?? string.Empty,
                Family = principal.FindFirstValue("family") ?? string.Empty,
                TokenId = principal.FindFirstValue(JwtRegisteredClaimNames.Jti) ?? string.Empty
            };
        }
        catch
        {
            return null;
        }
    }

    private TokenValidationParameters GetValidationParameters()
    {
        return new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _issuer,
            ValidateAudience = true,
            ValidAudience = _audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = _signingKey,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    }
}
