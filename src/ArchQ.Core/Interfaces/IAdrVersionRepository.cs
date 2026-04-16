using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAdrVersionRepository
{
    Task<AdrVersion> CreateAsync(AdrVersion version, string tenantSlug);
    Task<List<AdrVersion>> ListByAdrAsync(string adrId, string tenantSlug);
    Task<AdrVersion?> GetByVersionAsync(string adrId, int version, string tenantSlug);
}
