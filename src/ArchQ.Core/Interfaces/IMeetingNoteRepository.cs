using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IMeetingNoteRepository
{
    Task<MeetingNote> CreateAsync(MeetingNote note, string tenantSlug);
    Task<MeetingNote?> GetByIdAsync(string id, string tenantSlug);
    Task<List<MeetingNote>> ListByAdrAsync(string adrId, string tenantSlug);
    Task<MeetingNote> UpdateAsync(MeetingNote note, string tenantSlug);
    Task DeleteAsync(string id, string tenantSlug);
}
