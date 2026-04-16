using ArchQ.Application.Artifacts.Commands.AddMeetingNote;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetMeetingNotes;

public class GetMeetingNotesQueryHandler : IRequestHandler<GetMeetingNotesQuery, GetMeetingNotesResponse>
{
    private readonly IMeetingNoteRepository _repository;

    public GetMeetingNotesQueryHandler(IMeetingNoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetMeetingNotesResponse> Handle(GetMeetingNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await _repository.ListByAdrAsync(request.AdrId, request.TenantSlug);

        return new GetMeetingNotesResponse
        {
            Items = notes.Select(n => new MeetingNoteResponse
            {
                Id = n.Id,
                AdrId = n.AdrId,
                Title = n.Title,
                MeetingDate = n.MeetingDate,
                Attendees = n.Attendees,
                Body = n.Body,
                AuthorId = n.AuthorId,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            }).ToList()
        };
    }
}
