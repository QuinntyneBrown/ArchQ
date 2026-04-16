namespace ArchQ.Core.Entities;

public class Comment
{
    public string Type { get; set; } = "comment";
    public string Id { get; set; } = $"comment::{Guid.NewGuid()}";
    public string AdrId { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public bool Edited { get; set; }
    public bool Deleted { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
