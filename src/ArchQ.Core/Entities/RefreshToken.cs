namespace ArchQ.Core.Entities;

public class RefreshToken
{
    public string Type { get; set; } = "refresh_token";
    public string TokenHash { get; set; } = string.Empty;
    public string Family { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public DateTime IssuedAt { get; set; } = DateTime.UtcNow;
    public DateTime ExpiresAt { get; set; }
    public bool Revoked { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? ReplacedBy { get; set; }
}
