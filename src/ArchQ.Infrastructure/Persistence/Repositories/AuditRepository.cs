using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AuditRepository : IAuditRepository
{
    private readonly CouchbaseContext _context;
    private const string ScopeName = "system";
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

    public async Task<AuditListResult> ListAsync(string tenantSlug, AuditFilterParams filters, int pageSize, int offset)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var queryOptions = new QueryOptions();
        var whereClauses = new List<string> { "a.tenantId = $tenantSlug" };
        queryOptions.Parameter("tenantSlug", tenantSlug);

        if (!string.IsNullOrEmpty(filters.Action))
        {
            whereClauses.Add("a.action = $action");
            queryOptions.Parameter("action", filters.Action);
        }

        if (!string.IsNullOrEmpty(filters.ResourceId))
        {
            whereClauses.Add("a.entityId = $resourceId");
            queryOptions.Parameter("resourceId", filters.ResourceId);
        }

        if (!string.IsNullOrEmpty(filters.ActorId))
        {
            whereClauses.Add("a.userId = $actorId");
            queryOptions.Parameter("actorId", filters.ActorId);
        }

        if (filters.DateFrom.HasValue)
        {
            whereClauses.Add("a.timestamp >= $dateFrom");
            queryOptions.Parameter("dateFrom", filters.DateFrom.Value.ToString("o"));
        }

        if (filters.DateTo.HasValue)
        {
            whereClauses.Add("a.timestamp <= $dateTo");
            queryOptions.Parameter("dateTo", filters.DateTo.Value.ToString("o"));
        }

        var whereClause = string.Join(" AND ", whereClauses);

        // Count query
        var countQuery = $"SELECT COUNT(*) AS cnt FROM `{CollectionName}` a WHERE {whereClause}";
        var countResult = await scope.QueryAsync<dynamic>(countQuery, queryOptions);
        int totalCount = 0;
        await foreach (var row in countResult.Rows)
        {
            totalCount = (int)(long)row.cnt;
        }

        // Data query
        var dataQuery = $"SELECT a.* FROM `{CollectionName}` a WHERE {whereClause} ORDER BY a.timestamp DESC LIMIT {pageSize} OFFSET {offset}";
        var dataResult = await scope.QueryAsync<AuditEntry>(dataQuery, queryOptions);
        var items = new List<AuditEntry>();
        await foreach (var row in dataResult.Rows)
        {
            items.Add(row);
        }

        return new AuditListResult
        {
            Items = items,
            TotalCount = totalCount
        };
    }

    public async Task<AuditEntry?> GetByIdAsync(string id, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var query = $"SELECT a.* FROM `{CollectionName}` a WHERE a.id = $id AND a.tenantId = $tenantSlug LIMIT 1";
        var options = new QueryOptions()
            .Parameter("id", id)
            .Parameter("tenantSlug", tenantSlug);
        var result = await scope.QueryAsync<AuditEntry>(query, options);
        await foreach (var row in result.Rows)
        {
            return row;
        }
        return null;
    }

    public async Task<List<AuditEntry>> ListByResourceAsync(string resourceId, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(ScopeName);
        var query = $"SELECT a.* FROM `{CollectionName}` a WHERE a.entityId = $resourceId AND a.tenantId = $tenantSlug ORDER BY a.timestamp DESC";
        var options = new QueryOptions()
            .Parameter("resourceId", resourceId)
            .Parameter("tenantSlug", tenantSlug);
        var result = await scope.QueryAsync<AuditEntry>(query, options);
        var items = new List<AuditEntry>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }
        return items;
    }
}
