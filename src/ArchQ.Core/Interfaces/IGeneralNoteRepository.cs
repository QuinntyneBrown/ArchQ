using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IGeneralNoteRepository
{
    Task<GeneralNote> CreateAsync(GeneralNote note, string tenantSlug);
    Task<GeneralNote?> GetByIdAsync(string id, string tenantSlug);
    Task<List<GeneralNote>> ListByAdrAsync(string adrId, string tenantSlug);
    Task<GeneralNote> UpdateAsync(GeneralNote note, string tenantSlug);
    Task DeleteAsync(string id, string tenantSlug);
}
