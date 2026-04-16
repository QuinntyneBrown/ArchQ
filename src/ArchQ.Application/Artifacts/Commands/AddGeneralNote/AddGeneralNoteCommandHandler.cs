using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddGeneralNote;

public class AddGeneralNoteCommandHandler : IRequestHandler<AddGeneralNoteCommand, GeneralNoteResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IGeneralNoteRepository _noteRepository;
    private readonly IAuditRepository _auditRepository;

    public AddGeneralNoteCommandHandler(
        IAdrRepository adrRepository,
        IGeneralNoteRepository noteRepository,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _noteRepository = noteRepository;
        _auditRepository = auditRepository;
    }

    public async Task<GeneralNoteResponse> Handle(AddGeneralNoteCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new InvalidOperationException($"ADR '{request.AdrId}' not found.");

        if (adr.Status != "draft" && adr.Status != "in_review")
        {
            throw new InvalidOperationException("Notes can only be added to ADRs in Draft or In Review status.");
        }

        var note = new GeneralNote
        {
            AdrId = request.AdrId,
            Title = request.Title,
            Body = request.Body,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _noteRepository.CreateAsync(note, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "note.created",
            EntityType = "GeneralNote",
            EntityId = created.Id,
            UserId = request.AuthorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Note '{created.Title}' added to ADR '{adr.AdrNumber}'."
        });

        return new GeneralNoteResponse
        {
            Id = created.Id,
            AdrId = created.AdrId,
            Title = created.Title,
            Body = created.Body,
            AuthorId = created.AuthorId,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
