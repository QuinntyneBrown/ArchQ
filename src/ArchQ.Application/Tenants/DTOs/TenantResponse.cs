using ArchQ.Core.Entities;

namespace ArchQ.Application.Tenants.DTOs;

public class TenantResponse
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string Plan { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public TenantSettings Settings { get; set; } = new();

    public static TenantResponse FromEntity(Tenant tenant) => new()
    {
        Id = tenant.Id,
        DisplayName = tenant.DisplayName,
        Slug = tenant.Slug,
        Status = tenant.Status,
        Plan = tenant.Plan,
        CreatedAt = tenant.CreatedAt,
        UpdatedAt = tenant.UpdatedAt,
        Settings = tenant.Settings
    };
}
