using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAdrVersionRepository
{
    Task<AdrVersion> CreateAsync(AdrVersion version, string tenantSlug);
}
