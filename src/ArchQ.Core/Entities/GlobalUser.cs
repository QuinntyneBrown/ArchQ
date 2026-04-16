namespace ArchQ.Core.Entities;

public class GlobalUser
{
    public string Type { get; set; } = "global_user";
    public string Email { get; set; } = string.Empty;
    public List<GlobalUserMembership> Memberships { get; set; } = new();
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}

public class GlobalUserMembership
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
