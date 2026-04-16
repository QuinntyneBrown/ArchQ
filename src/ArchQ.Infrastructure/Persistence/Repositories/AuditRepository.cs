using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "_system";
    private const string CollectionName = "audit";

    public AuditRepository(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task WriteEntryAsync(AuditEntry entry)
    {
        var collection = await _context.GetCollectionAsync(ScopeName, CollectionName);
        var key = $"audit::{entry.Timestamp:yyyyMMddHHmmss}::{entry.Id}";
        await collection.InsertAsync(key, entry);
    }
}
