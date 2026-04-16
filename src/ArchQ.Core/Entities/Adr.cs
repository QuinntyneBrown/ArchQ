namespace ArchQ.Core.Entities;

public class Adr
{
    public string Type { get; set; } = "adr";
    public string Id { get; set; } = $"adr::{Guid.NewGuid()}";
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = "draft";
    public string AuthorId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Version { get; set; } = 1;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
