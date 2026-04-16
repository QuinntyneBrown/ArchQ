using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddMeetingNote;

public class AddMeetingNoteCommandHandler : IRequestHandler<AddMeetingNoteCommand, MeetingNoteResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IMeetingNoteRepository _meetingNoteRepository;
    private readonly IAuditRepository _auditRepository;

    public AddMeetingNoteCommandHandler(
        IAdrRepository adrRepository,
        IMeetingNoteRepository meetingNoteRepository,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _meetingNoteRepository = meetingNoteRepository;
        _auditRepository = auditRepository;
    }

    public async Task<MeetingNoteResponse> Handle(AddMeetingNoteCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new InvalidOperationException($"ADR '{request.AdrId}' not found.");

        if (adr.Status != "draft" && adr.Status != "in_review")
        {
            throw new InvalidOperationException("Meeting notes can only be added to ADRs in Draft or In Review status.");
        }

        var note = new MeetingNote
        {
            AdrId = request.AdrId,
            Title = request.Title,
            MeetingDate = request.MeetingDate,
            Attendees = request.Attendees,
            Body = request.Body,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _meetingNoteRepository.CreateAsync(note, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "meeting_note.created",
            EntityType = "MeetingNote",
            EntityId = created.Id,
            UserId = request.AuthorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Meeting note '{created.Title}' added to ADR '{adr.AdrNumber}'."
        });

        return new MeetingNoteResponse
        {
            Id = created.Id,
            AdrId = created.AdrId,
            Title = created.Title,
            MeetingDate = created.MeetingDate,
            Attendees = created.Attendees,
            Body = created.Body,
            AuthorId = created.AuthorId,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
