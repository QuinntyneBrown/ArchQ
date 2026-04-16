using ArchQ.Core.Interfaces;

namespace ArchQ.Infrastructure.Identity;

public class TenantContext : ITenantContext
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
}
