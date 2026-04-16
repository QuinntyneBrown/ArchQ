using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface ITagRepository
{
    Task<Tag> CreateAsync(Tag tag, string tenantSlug);
    Task<Tag?> GetBySlugAsync(string slug, string tenantSlug);
    Task<List<Tag>> ListAllAsync(string tenantSlug);
    Task<List<Tag>> SearchAsync(string query, string tenantSlug);
    Task IncrementUsageAsync(string slug, string tenantSlug);
    Task DecrementUsageAsync(string slug, string tenantSlug);
}
