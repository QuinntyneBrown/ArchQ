namespace ArchQ.Core.Entities;

public class AdrApprover
{
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = "pending";
    public string? Comment { get; set; }
    public DateTime? DecidedAt { get; set; }
}
