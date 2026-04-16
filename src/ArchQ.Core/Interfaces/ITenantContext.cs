namespace ArchQ.Core.Interfaces;

public interface ITenantContext
{
    string TenantId { get; }
    string TenantSlug { get; }
}
