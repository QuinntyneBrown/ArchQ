namespace ArchQ.Core.Entities;

public class AdrSummary
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
