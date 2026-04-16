using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.UpdateAdr;

public class UpdateAdrCommandHandler : IRequestHandler<UpdateAdrCommand, UpdateAdrResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IAdrVersionRepository _adrVersionRepository;
    private readonly IAuditRepository _auditRepository;

    public UpdateAdrCommandHandler(
        IAdrRepository adrRepository,
        IAdrVersionRepository adrVersionRepository,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _adrVersionRepository = adrVersionRepository;
        _auditRepository = auditRepository;
    }

    public async Task<UpdateAdrResponse> Handle(UpdateAdrCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new NotFoundException("ADR_NOT_FOUND", $"ADR '{request.AdrId}' was not found.");

        // Assert editable: status must be draft or in_review
        if (adr.Status != "draft" && adr.Status != "in_review")
        {
            throw new ForbiddenException("ADR_NOT_EDITABLE", $"ADR is in '{adr.Status}' status and cannot be edited.");
        }

        // Assert editable: actor must be author or admin
        var isAuthor = request.ActorId == adr.AuthorId;
        var isAdmin = request.ActorRoles.Contains("admin");
        if (!isAuthor && !isAdmin)
        {
            throw new ForbiddenException("ADR_EDIT_FORBIDDEN", "You do not have permission to edit this ADR.");
        }

        var previousVersion = adr.Version;

        // Archive current version
        var adrVersion = new AdrVersion
        {
            AdrId = adr.Id,
            Version = adr.Version,
            Title = adr.Title,
            Body = adr.Body,
            EditedBy = request.ActorId,
            CreatedAt = DateTime.UtcNow
        };
        await _adrVersionRepository.CreateAsync(adrVersion, request.TenantSlug);

        // Update ADR fields
        adr.Title = request.Title;
        adr.Body = request.Body;
        adr.Tags = request.Tags;
        adr.Version++;
        adr.UpdatedAt = DateTime.UtcNow;

        var updated = await _adrRepository.UpdateAsync(adr, request.TenantSlug);

        // Write audit entry
        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "adr.updated",
            EntityType = "Adr",
            EntityId = updated.Id,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"ADR '{updated.AdrNumber}: {updated.Title}' updated from version {previousVersion} to {updated.Version}."
        });

        return new UpdateAdrResponse
        {
            Id = updated.Id,
            AdrNumber = updated.AdrNumber,
            Title = updated.Title,
            Status = updated.Status,
            Version = updated.Version,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
