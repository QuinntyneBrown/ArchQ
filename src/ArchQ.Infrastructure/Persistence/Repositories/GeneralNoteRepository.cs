using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class GeneralNoteRepository : IGeneralNoteRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "notes";

    public GeneralNoteRepository(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task<GeneralNote> CreateAsync(GeneralNote note, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(note.Id, note);
        return note;
    }

    public async Task<GeneralNote?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(id);
            return result.ContentAs<GeneralNote>();
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<GeneralNote>> ListByAdrAsync(string adrId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT n.* FROM `{CollectionName}` n WHERE n.type = \"note\" AND n.adrId = $adrId ORDER BY n.createdAt DESC";
        var options = new QueryOptions().Parameter("adrId", adrId);

        var result = await scope.QueryAsync<GeneralNote>(query, options);
        var items = new List<GeneralNote>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }
        return items;
    }

    public async Task<GeneralNote> UpdateAsync(GeneralNote note, string tenantSlug)
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
