using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AttachmentRepository : IAttachmentRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "attachments_meta";

    public AttachmentRepository(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task<AttachmentMeta> CreateAsync(AttachmentMeta attachment, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(attachment.Id, attachment);
        return attachment;
    }

    public async Task<AttachmentMeta?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(id);
            return result.ContentAs<AttachmentMeta>();
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<AttachmentMeta>> ListByAdrAsync(string adrId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT a.* FROM `{CollectionName}` a WHERE a.type = \"attachment\" AND a.adrId = $adrId ORDER BY a.createdAt DESC";
        var options = new QueryOptions().Parameter("adrId", adrId);

        var result = await scope.QueryAsync<AttachmentMeta>(query, options);
        var items = new List<AttachmentMeta>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }
        return items;
    }

    public async Task<AttachmentMeta> UpdateAsync(AttachmentMeta attachment, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        var getResult = await collection.GetAsync(attachment.Id);
        await collection.ReplaceAsync(attachment.Id, attachment, new Couchbase.KeyValue.ReplaceOptions().Cas(getResult.Cas));
        return attachment;
    }

    public async Task DeleteAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.RemoveAsync(id);
    }
}
