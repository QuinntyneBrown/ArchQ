namespace ArchQ.Application.Adrs.Queries.ListAdrs;

public class AdrListResponse
{
    public List<AdrListItemResponse> Items { get; set; } = new();
    public string? NextCursor { get; set; }
    public string? PrevCursor { get; set; }
    public int TotalCount { get; set; }
}

public class AdrListItemResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
