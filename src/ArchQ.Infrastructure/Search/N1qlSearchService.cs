using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using ArchQ.Infrastructure.Persistence;
using Couchbase.Query;

namespace ArchQ.Infrastructure.Search;

// TODO: Migrate to Couchbase Full-Text Search (FTS) when available for richer
// relevance scoring, fuzzy matching, and better performance on large datasets.
public class N1qlSearchService : ISearchService
{
    private readonly CouchbaseContext _context;
    private const string CollectionName = "adrs";
    private const int SnippetLength = 150;

    public N1qlSearchService(CouchbaseContext context)
    {
        _context = context;
    }

    public async Task<SearchResult> SearchAsync(string query, string tenantSlug, string? status, int pageSize, int offset)
    {
        var sw = Stopwatch.StartNew();
        var scope = await _context.GetScopeAsync(tenantSlug);

        var lowerQuery = query.ToLowerInvariant();
        var likePattern = $"%{EscapeLike(lowerQuery)}%";

        var queryOptions = new QueryOptions();
        queryOptions.Parameter("likePattern", likePattern);
        queryOptions.Parameter("lowerQuery", lowerQuery);

        var whereClauses = new List<string>
        {
            "a.type = \"adr\"",
            "(LOWER(a.title) LIKE $likePattern OR LOWER(a.body) LIKE $likePattern OR ANY t IN a.tags SATISFIES LOWER(t) LIKE $likePattern END)"
        };

        if (!string.IsNullOrEmpty(status))
        {
            whereClauses.Add("a.status = $status");
            queryOptions.Parameter("status", status);
        }

        var whereClause = string.Join(" AND ", whereClauses);

        // Count query
        var countQuery = $"SELECT COUNT(*) AS cnt FROM `{CollectionName}` a WHERE {whereClause}";
        var countResult = await scope.QueryAsync<dynamic>(countQuery, queryOptions);
        var totalHits = 0;
        await foreach (var row in countResult.Rows)
        {
            totalHits = (int)(long)row.cnt;
        }

        // Data query
        var dataQuery = $"SELECT a.id, a.adrNumber, a.title, a.body, a.status, a.tags FROM `{CollectionName}` a WHERE {whereClause} ORDER BY a.updatedAt DESC LIMIT {pageSize} OFFSET {offset}";
        var dataResult = await scope.QueryAsync<SearchRawRow>(dataQuery, queryOptions);

        var results = new List<SearchResultItem>();
        await foreach (var row in dataResult.Rows)
        {
            var score = ComputeScore(row, lowerQuery);
            var snippet = ExtractSnippet(row.Body ?? string.Empty, lowerQuery);
            var highlightedTitle = HighlightTitle(row.Title ?? string.Empty, lowerQuery);

            results.Add(new SearchResultItem
            {
                Id = row.Id,
                AdrNumber = row.AdrNumber,
                Title = highlightedTitle,
                Status = row.Status,
                Score = score,
                Snippet = snippet
            });
        }

        // Sort by score descending (secondary sort already handled by updatedAt in query)
        results = results.OrderByDescending(r => r.Score).ToList();

        sw.Stop();

        return new SearchResult
        {
            Results = results,
            TotalHits = totalHits,
            Took = sw.ElapsedMilliseconds
        };
    }

    private static double ComputeScore(SearchRawRow row, string lowerQuery)
    {
        double score = 0;

        if (!string.IsNullOrEmpty(row.Title) && row.Title.ToLowerInvariant().Contains(lowerQuery))
            score += 3;

        if (row.Tags is { Count: > 0 } && row.Tags.Any(t => t.ToLowerInvariant().Contains(lowerQuery)))
            score += 2;

        if (!string.IsNullOrEmpty(row.Body) && row.Body.ToLowerInvariant().Contains(lowerQuery))
            score += 1;

        return score;
    }

    private static string ExtractSnippet(string body, string lowerQuery)
    {
        if (string.IsNullOrEmpty(body))
            return string.Empty;

        var lowerBody = body.ToLowerInvariant();
        var matchIndex = lowerBody.IndexOf(lowerQuery, StringComparison.Ordinal);

        if (matchIndex < 0)
            return body.Length <= SnippetLength ? body : body[..SnippetLength] + "...";

        var halfWindow = SnippetLength / 2;
        var start = Math.Max(0, matchIndex - halfWindow);
        var end = Math.Min(body.Length, start + SnippetLength);
        if (end - start < SnippetLength && start > 0)
            start = Math.Max(0, end - SnippetLength);

        var snippet = body[start..end];

        var prefix = start > 0 ? "..." : "";
        var suffix = end < body.Length ? "..." : "";

        // Wrap the matching text with <mark> tags in the snippet
        var escapedQuery = Regex.Escape(lowerQuery);
        snippet = Regex.Replace(snippet, escapedQuery, m => $"<mark>{m.Value}</mark>", RegexOptions.IgnoreCase);

        return $"{prefix}{snippet}{suffix}";
    }

    private static string HighlightTitle(string title, string lowerQuery)
    {
        if (string.IsNullOrEmpty(title))
            return title;

        var escapedQuery = Regex.Escape(lowerQuery);
        return Regex.Replace(title, escapedQuery, m => $"<mark>{m.Value}</mark>", RegexOptions.IgnoreCase);
    }

    private static string EscapeLike(string input)
    {
        // Escape N1QL LIKE special characters
        return input
            .Replace("\\", "\\\\")
            .Replace("%", "\\%")
            .Replace("_", "\\_");
    }

    private class SearchRawRow
    {
        public string Id { get; set; } = string.Empty;
        public string AdrNumber { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public List<string>? Tags { get; set; }
    }
}
