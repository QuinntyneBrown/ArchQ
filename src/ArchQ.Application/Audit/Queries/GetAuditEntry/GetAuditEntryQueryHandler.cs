using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Audit.Queries.GetAuditEntry;

public class GetAuditEntryQueryHandler : IRequestHandler<GetAuditEntryQuery, GetAuditEntryResponse>
{
    private readonly IAuditRepository _auditRepository;

    public GetAuditEntryQueryHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<GetAuditEntryResponse> Handle(GetAuditEntryQuery request, CancellationToken cancellationToken)
    {
        var entry = await _auditRepository.GetByIdAsync(request.Id, request.TenantSlug)
            ?? throw new NotFoundException("AUDIT_NOT_FOUND", "Audit entry not found.");

        return new GetAuditEntryResponse
        {
            Id = entry.Id,
            Action = entry.Action,
            EntityType = entry.EntityType,
            EntityId = entry.EntityId,
            UserId = entry.UserId,
            Timestamp = entry.Timestamp,
            Details = entry.Details,
            IpAddress = entry.IpAddress,
            UserAgent = entry.UserAgent
        };
    }
}
