using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface ITenantRepository
{
    Task<Tenant?> GetByIdAsync(string id);
    Task<Tenant?> GetBySlugAsync(string slug);
    Task<Tenant> CreateAsync(Tenant tenant);
    Task<Tenant> UpdateAsync(Tenant tenant);
    Task<bool> SlugExistsAsync(string slug);
    Task SoftDeleteAsync(string id);
}
