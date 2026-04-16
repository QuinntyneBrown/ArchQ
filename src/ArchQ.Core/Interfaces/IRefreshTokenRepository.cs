using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken refreshToken);
    Task<RefreshToken?> GetByTokenHashAsync(string hash);
    Task UpdateAsync(RefreshToken refreshToken);
    Task RevokeAllInFamilyAsync(string family);
}
