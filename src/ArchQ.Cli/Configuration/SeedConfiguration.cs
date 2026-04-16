namespace ArchQ.Cli.Configuration;

public class SeedConfiguration
{
    public string AdminEmail { get; set; } = "admin@archq.local";
    public string AdminPassword { get; set; } = "Admin@123";
    public string AdminName { get; set; } = "System Administrator";
    public string TenantName { get; set; } = "Default Organization";
    public string TenantSlug { get; set; } = "default";
}
