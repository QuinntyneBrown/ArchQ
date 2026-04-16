namespace ArchQ.Core.Entities;

public class Tag
{
    public string Type { get; set; } = "tag";
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public static string GenerateId(string slug) => $"tag::{slug}";
}
