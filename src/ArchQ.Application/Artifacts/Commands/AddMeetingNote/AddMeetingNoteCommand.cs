using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddMeetingNote;

public class AddMeetingNoteCommand : IRequest<MeetingNoteResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Attendees { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
}

public class MeetingNoteResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Attendees { get; set; } = new();
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
