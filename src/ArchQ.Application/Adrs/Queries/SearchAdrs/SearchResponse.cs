namespace ArchQ.Application.Adrs.Queries.SearchAdrs;

public class SearchResponse
{
    public List<SearchResultItemResponse> Results { get; set; } = new();
    public int TotalHits { get; set; }
    public long Took { get; set; }
}

public class SearchResultItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public double Score { get; set; }
    public string Snippet { get; set; } = string.Empty;
}
