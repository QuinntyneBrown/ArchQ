using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class CommentRepository : ICommentRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "comments";

    public CommentRepository(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task<Comment> CreateAsync(Comment comment, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.InsertAsync(comment.Id, comment);
        return comment;
    }

    public async Task<Comment?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(id);
            return result.ContentAs<Comment>();
        }
        catch (DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<List<Comment>> ListByAdrAsync(string adrId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT c.* FROM `{CollectionName}` c WHERE c.type = \"comment\" AND c.adrId = $adrId ORDER BY c.createdAt ASC";
        var options = new QueryOptions().Parameter("adrId", adrId);

        var result = await scope.QueryAsync<Comment>(query, options);
        var items = new List<Comment>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }
        return items;
    }

    public async Task<Comment> UpdateAsync(Comment comment, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        var getResult = await collection.GetAsync(comment.Id);
        await collection.ReplaceAsync(comment.Id, comment, new Couchbase.KeyValue.ReplaceOptions().Cas(getResult.Cas));
        return comment;
    }

    public async Task DeleteAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        await collection.RemoveAsync(id);
    }
}
