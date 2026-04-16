using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAdrRepository
{
    Task<Adr> CreateAsync(Adr adr, string tenantSlug);
    Task<Adr?> GetByIdAsync(string id, string tenantSlug);
    Task<int> GetMaxAdrNumberAsync(string tenantSlug);
    Task<Adr> UpdateAsync(Adr adr, string tenantSlug);
}
