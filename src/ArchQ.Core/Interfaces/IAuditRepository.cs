using ArchQ.Core.Entities;

namespace ArchQ.Core.Interfaces;

public interface IAuditRepository
{
    Task WriteEntryAsync(AuditEntry entry);
}
