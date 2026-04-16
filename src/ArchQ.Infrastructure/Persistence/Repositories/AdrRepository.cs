using System.Text;
using System.Text.Json;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using Couchbase.KeyValue;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Persistence.Repositories;

public class AdrRepository : IAdrRepository
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "adrs";
    private const int MaxCasRetries = 3;

    private static readonly HashSet<string> AllowedSortFields = new(StringComparer.OrdinalIgnoreCase)
    {
        "updatedAt", "createdAt", "adrNumber", "title", "status"
    };

    public AdrRepository(CouchbaseContext context)
    {
        _context = context;
    }

    // Id already carries the "adr::" prefix (e.g. "adr::550e8400-...")
    private static string DocKey(string id) => id;

    public async Task<Adr> CreateAsync(Adr adr, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);

        for (int attempt = 0; attempt <= MaxCasRetries; attempt++)
        {
            try
            {
                await collection.InsertAsync(DocKey(adr.Id), adr);
                return adr;
            }
            catch (DocumentExistsException) when (attempt < MaxCasRetries)
            {
                // Concurrent insert conflict — re-generate id and re-sequence the ADR number
                adr.Id = $"adr::{Guid.NewGuid()}";
                var maxNum = await GetMaxAdrNumberAsync(tenantSlug);
                adr.AdrNumber = $"ADR-{(maxNum + 1):D3}";
            }
        }

        throw new InvalidOperationException("Failed to insert ADR after maximum CAS retries due to concurrent creation conflicts.");
    }

    public async Task<Adr?> GetByIdAsync(string id, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);
        try
        {
            var result = await collection.GetAsync(DocKey(id));
            return result.ContentAs<Adr>();
        }
        catch (Couchbase.Core.Exceptions.KeyValue.DocumentNotFoundException)
        {
            return null;
        }
    }

    public async Task<Adr> UpdateAsync(Adr adr, string tenantSlug)
    {
        var collection = await _context.GetCollectionAsync(tenantSlug, CollectionName);

        for (int attempt = 0; attempt <= MaxCasRetries; attempt++)
        {
            try
            {
                var getResult = await collection.GetAsync(DocKey(adr.Id));
                var cas = getResult.Cas;
                await collection.ReplaceAsync(DocKey(adr.Id), adr, new Couchbase.KeyValue.ReplaceOptions().Cas(cas));
                return adr;
            }
            catch (DocumentExistsException) when (attempt < MaxCasRetries)
            {
                // Retry on CAS conflict
            }
        }

        throw new InvalidOperationException("Failed to update ADR after maximum CAS retries.");
    }

    public async Task<AdrListResult> ListAsync(AdrListParams listParams, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var queryOptions = new QueryOptions();

        var whereClauses = new List<string> { "a.type = \"adr\"" };
        BuildFilterClauses(listParams, whereClauses, queryOptions);

        var sortField = AllowedSortFields.Contains(listParams.SortField) ? listParams.SortField : "updatedAt";
        var sortDir = string.Equals(listParams.SortDirection, "asc", StringComparison.OrdinalIgnoreCase) ? "ASC" : "DESC";

        // Decode cursor and add pagination WHERE clause
        if (!string.IsNullOrEmpty(listParams.Cursor))
        {
            try
            {
                var cursorJson = Encoding.UTF8.GetString(Convert.FromBase64String(listParams.Cursor));
                var cursor = JsonSerializer.Deserialize<CursorData>(cursorJson);
                if (cursor != null)
                {
                    var op = sortDir == "ASC" ? ">" : "<";
                    whereClauses.Add($"(a.{sortField} {op} $cursorSortValue OR (a.{sortField} = $cursorSortValue AND META(a).id {op} $cursorDocId))");
                    queryOptions.Parameter("cursorSortValue", cursor.SortValue);
                    queryOptions.Parameter("cursorDocId", cursor.DocId);
                }
            }
            catch
            {
                // Invalid cursor — ignore and return from the start
            }
        }

        var whereClause = string.Join(" AND ", whereClauses);
        var fetchLimit = listParams.PageSize + 1;

        var query = $"SELECT a.id, a.adrNumber, a.title, a.status, a.authorId, a.tags, a.updatedAt FROM `{CollectionName}` a WHERE {whereClause} ORDER BY a.{sortField} {sortDir}, META(a).id {sortDir} LIMIT {fetchLimit}";

        var result = await scope.QueryAsync<AdrSummary>(query, queryOptions);
        var items = new List<AdrSummary>();
        await foreach (var row in result.Rows)
        {
            items.Add(row);
        }

        string? nextCursor = null;
        if (items.Count > listParams.PageSize)
        {
            items.RemoveAt(items.Count - 1);
            var lastItem = items[^1];
            var cursorData = new CursorData
            {
                SortValue = GetSortValue(lastItem, sortField),
                DocId = lastItem.Id
            };
            nextCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cursorData)));
        }

        // PrevCursor: if a cursor was provided, the first item can serve as prev reference
        string? prevCursor = null;
        if (!string.IsNullOrEmpty(listParams.Cursor) && items.Count > 0)
        {
            var firstItem = items[0];
            var cursorData = new CursorData
            {
                SortValue = GetSortValue(firstItem, sortField),
                DocId = firstItem.Id
            };
            prevCursor = Convert.ToBase64String(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(cursorData)));
        }

        return new AdrListResult
        {
            Items = items,
            NextCursor = nextCursor,
            PrevCursor = prevCursor,
            TotalCount = 0 // Caller will use CountAsync separately
        };
    }

    public async Task<int> CountAsync(AdrListParams filters, string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var queryOptions = new QueryOptions();

        var whereClauses = new List<string> { "a.type = \"adr\"" };
        BuildFilterClauses(filters, whereClauses, queryOptions);

        var whereClause = string.Join(" AND ", whereClauses);
        var query = $"SELECT COUNT(*) AS cnt FROM `{CollectionName}` a WHERE {whereClause}";

        var result = await scope.QueryAsync<dynamic>(query, queryOptions);
        await foreach (var row in result.Rows)
        {
            return (int)(long)row.cnt;
        }

        return 0;
    }

    private static void BuildFilterClauses(AdrListParams listParams, List<string> whereClauses, QueryOptions queryOptions)
    {
        if (!string.IsNullOrEmpty(listParams.Status))
        {
            whereClauses.Add("a.status = $status");
            queryOptions.Parameter("status", listParams.Status);
        }

        if (!string.IsNullOrEmpty(listParams.AuthorId))
        {
            whereClauses.Add("a.authorId = $authorId");
            queryOptions.Parameter("authorId", listParams.AuthorId);
        }

        if (listParams.Tags is { Count: > 0 })
        {
            whereClauses.Add("ANY t IN a.tags SATISFIES t IN $tags END");
            queryOptions.Parameter("tags", listParams.Tags);
        }

        if (listParams.DateFrom.HasValue)
        {
            whereClauses.Add("a.updatedAt >= $dateFrom");
            queryOptions.Parameter("dateFrom", listParams.DateFrom.Value.ToString("o"));
        }

        if (listParams.DateTo.HasValue)
        {
            whereClauses.Add("a.updatedAt <= $dateTo");
            queryOptions.Parameter("dateTo", listParams.DateTo.Value.ToString("o"));
        }
    }

    private static string GetSortValue(AdrSummary item, string sortField)
    {
        return sortField.ToLowerInvariant() switch
        {
            "updatedat" => item.UpdatedAt.ToString("o"),
            "createdat" => item.UpdatedAt.ToString("o"), // summaries only carry UpdatedAt
            "adrnumber" => item.AdrNumber,
            "title" => item.Title,
            "status" => item.Status,
            _ => item.UpdatedAt.ToString("o")
        };
    }

    private class CursorData
    {
        public string SortValue { get; set; } = string.Empty;
        public string DocId { get; set; } = string.Empty;
    }

    public async Task<int> GetMaxAdrNumberAsync(string tenantSlug)
    {
        var scope = await _context.GetScopeAsync(tenantSlug);
        var query = $"SELECT MAX(TONUMBER(REPLACE(a.adrNumber, \"ADR-\", \"\"))) AS maxNum FROM `{CollectionName}` a WHERE a.type = \"adr\"";
        var result = await scope.QueryAsync<dynamic>(query);

        await foreach (var row in result.Rows)
        {
            if (row.maxNum is not null)
            {
                return (int)(long)row.maxNum;
            }
        }

        return 0;
    }
}
