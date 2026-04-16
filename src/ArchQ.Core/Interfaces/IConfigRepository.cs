using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IConfigRepository
{
    Task<TemplateConfig?> GetByKeyAsync(string key, string tenantSlug);
    Task UpsertAsync(TemplateConfig config, string tenantSlug);
}
