namespace ArchQ.Core.Entities;

public class AdrVersion
{
    public string Type { get; set; } = "adr_version";
    public string Id { get; set; } = $"adr_version::{Guid.NewGuid()}";
    public string AdrId { get; set; } = string.Empty;
    public int Version { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string EditedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
