using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface ISearchService
{
    Task<SearchResult> SearchAsync(string query, string tenantSlug, string? status, int pageSize, int offset);
}
