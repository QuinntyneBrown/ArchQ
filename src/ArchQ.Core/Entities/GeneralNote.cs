namespace ArchQ.Core.Entities;

public class GeneralNote
{
    public string Type { get; set; } = "note";
    public string Id { get; set; } = $"note::{Guid.NewGuid()}";
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
