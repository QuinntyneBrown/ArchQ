using ArchQ.Application.Adrs.DTOs;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.CreateAdr;

public class CreateAdrCommandHandler : IRequestHandler<CreateAdrCommand, AdrResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IAuditRepository _auditRepository;

    public CreateAdrCommandHandler(IAdrRepository adrRepository, IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _auditRepository = auditRepository;
    }

    public async Task<AdrResponse> Handle(CreateAdrCommand request, CancellationToken cancellationToken)
    {
        var maxNumber = await _adrRepository.GetMaxAdrNumberAsync(request.TenantSlug);
        var nextNumber = maxNumber + 1;
        var adrNumber = $"ADR-{nextNumber:D3}";

        var adr = new Adr
        {
            Title = request.Title,
            Body = request.Body,
            Tags = request.Tags,
            AuthorId = request.AuthorId,
            AdrNumber = adrNumber,
            Status = "draft",
            Version = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _adrRepository.CreateAsync(adr, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "adr.created",
            EntityType = "Adr",
            EntityId = created.Id,
            UserId = request.AuthorId,
            Timestamp = DateTime.UtcNow,
            Details = $"ADR '{created.AdrNumber}: {created.Title}' created."
        });

        return AdrResponse.FromEntity(created);
    }
}
