using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public class AccessTokenPayload
{
    public string UserId { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string TenantId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class RefreshTokenPayload
{
    public string UserId { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public string TokenId { get; set; } = string.Empty;
}

public interface ITokenService
{
    string CreateAccessToken(User user, string tenantId, string tenantSlug, List<string> roles);
    string CreateRefreshToken(string userId, string family);
    AccessTokenPayload? VerifyAccessToken(string token);
    RefreshTokenPayload? VerifyRefreshToken(string token);
}
