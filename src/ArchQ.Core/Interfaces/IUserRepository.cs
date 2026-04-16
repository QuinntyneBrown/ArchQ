using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IUserRepository
{
    Task<User> CreateAsync(User user, string tenantSlug);
    Task<User?> GetByIdAsync(string id, string tenantSlug);
    Task<User?> GetByEmailAsync(string email, string tenantSlug);
    Task<User> UpdateAsync(User user, string tenantSlug);
    Task<int> CountByRoleAsync(string role, string tenantSlug);
}
