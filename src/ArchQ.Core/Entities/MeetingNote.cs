namespace ArchQ.Core.Entities;

public class MeetingNote
{
    public string Type { get; set; } = "meeting_note";
    public string Id { get; set; } = $"mnote::{Guid.NewGuid()}";
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Attendees { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
