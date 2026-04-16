namespace ArchQ.Core.Entities;

public class AdrListResult
{
    public List<AdrSummary> Items { get; set; } = new();
    public string? NextCursor { get; set; }
    public string? PrevCursor { get; set; }
    public int TotalCount { get; set; }
}
