using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface ICommentRepository
{
    Task<Comment> CreateAsync(Comment comment, string tenantSlug);
    Task<Comment?> GetByIdAsync(string id, string tenantSlug);
    Task<List<Comment>> ListByAdrAsync(string adrId, string tenantSlug);
    Task<Comment> UpdateAsync(Comment comment, string tenantSlug);
    Task DeleteAsync(string id, string tenantSlug);
}
