using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IGlobalUserRepository
{
    Task<GlobalUser?> GetByEmailAsync(string email);
    Task<GlobalUser> CreateAsync(GlobalUser globalUser);
    Task<GlobalUser> UpdateAsync(GlobalUser globalUser);
    Task<bool> EmailExistsAsync(string email);
}
