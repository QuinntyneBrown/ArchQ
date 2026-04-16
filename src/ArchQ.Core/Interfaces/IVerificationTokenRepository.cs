using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IVerificationTokenRepository
{
    Task CreateAsync(VerificationToken token);
    Task<VerificationToken?> GetByTokenHashAsync(string tokenHash);
    Task UpdateAsync(VerificationToken token);
}
