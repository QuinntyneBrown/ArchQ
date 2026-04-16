using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Audit.Queries.ListAuditEntries;

public class ListAuditEntriesQueryHandler : IRequestHandler<ListAuditEntriesQuery, ListAuditEntriesResponse>
{
    private readonly IAuditRepository _auditRepository;

    public ListAuditEntriesQueryHandler(IAuditRepository auditRepository)
    {
        _auditRepository = auditRepository;
    }

    public async Task<ListAuditEntriesResponse> Handle(ListAuditEntriesQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);
        var offset = Math.Max(request.Offset, 0);

        var filters = new AuditFilterParams
        {
            Action = request.Action,
            ResourceId = request.ResourceId,
            ActorId = request.ActorId,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo
        };

        var result = await _auditRepository.ListAsync(request.TenantSlug, filters, pageSize, offset);

        return new ListAuditEntriesResponse
        {
            Items = result.Items.Select(e => new AuditEntryItem
            {
                Id = e.Id,
                Action = e.Action,
                EntityType = e.EntityType,
                EntityId = e.EntityId,
                UserId = e.UserId,
                Timestamp = e.Timestamp,
                Details = e.Details
            }).ToList(),
            TotalCount = result.TotalCount,
            PageSize = pageSize,
            Offset = offset
        };
    }
}
