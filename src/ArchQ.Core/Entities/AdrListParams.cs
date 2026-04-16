namespace ArchQ.Core.Entities;

public class AdrListParams
{
    public string? Status { get; set; }
    public string? AuthorId { get; set; }
    public List<string>? Tags { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string SortField { get; set; } = "updatedAt";
    public string SortDirection { get; set; } = "desc";
    public string? Cursor { get; set; }
    public int PageSize { get; set; } = 25;
}
