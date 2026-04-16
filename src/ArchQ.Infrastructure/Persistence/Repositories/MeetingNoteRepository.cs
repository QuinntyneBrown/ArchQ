using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class MeetingNoteRepository : IMeetingNoteRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "meeting_notes";

    public MeetingNoteRepository(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task<MeetingNote> CreateAsync(MeetingNote note, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(note.Id, note);
        return note;
    }

    public async Task<MeetingNote?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(id);
            return result.ContentAs<MeetingNote>();
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<MeetingNote>> ListByAdrAsync(string adrId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT m.* FROM `{CollectionName}` m WHERE m.type = \"meeting_note\" AND m.adrId = $adrId ORDER BY m.meetingDate DESC";
        var options = new QueryOptions().Parameter("adrId", adrId);

        var result = await scope.QueryAsync<MeetingNote>(query, options);
        var items = new List<MeetingNote>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }
        return items;
    }

    public async Task<MeetingNote> UpdateAsync(MeetingNote note, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        var getResult = await collection.GetAsync(note.Id);
        await collection.ReplaceAsync(note.Id, note, new Couchbase.KeyValue.ReplaceOptions().Cas(getResult.Cas));
        return note;
    }

    public async Task DeleteAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.RemoveAsync(id);
    }
}
