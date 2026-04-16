using ArchQ.Application.Artifacts.Commands.AddMeetingNote;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetMeetingNotes;

public class GetMeetingNotesQuery : IRequest<GetMeetingNotesResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}

public class GetMeetingNotesResponse
{
    public List<MeetingNoteResponse> Items { get; set; } = new();
}
