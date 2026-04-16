using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAttachmentRepository
{
    Task<AttachmentMeta> CreateAsync(AttachmentMeta attachment, string tenantSlug);
    Task<AttachmentMeta?> GetByIdAsync(string id, string tenantSlug);
    Task<List<AttachmentMeta>> ListByAdrAsync(string adrId, string tenantSlug);
    Task<AttachmentMeta> UpdateAsync(AttachmentMeta attachment, string tenantSlug);
    Task DeleteAsync(string id, string tenantSlug);
}
