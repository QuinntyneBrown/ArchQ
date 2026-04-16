using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.RecordDecision;

public class RecordDecisionCommandHandler : IRequestHandler<RecordDecisionCommand, RecordDecisionResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IConfigRepository _configRepository;

    public RecordDecisionCommandHandler(
        IAdrRepository adrRepository,
        IAuditRepository auditRepository,
        IConfigRepository configRepository)
    {
        _adrRepository = adrRepository;
        _auditRepository = auditRepository;
        _configRepository = configRepository;
    }

    public async Task<RecordDecisionResponse> Handle(RecordDecisionCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new NotFoundException("NOT_FOUND", $"ADR '{request.AdrId}' not found.");

        if (!string.Equals(adr.Status, "in-review", StringComparison.OrdinalIgnoreCase))
        {
            throw new ValidationFailedException("INVALID_STATUS",
                $"ADR status must be 'in-review' to record a decision. Current status: '{adr.Status}'.");
        }

        var approver = adr.Approvers.FirstOrDefault(a =>
            string.Equals(a.UserId, request.UserId, StringComparison.OrdinalIgnoreCase));

        if (approver is null)
        {
            throw new ForbiddenException("NOT_ASSIGNED_APPROVER",
                $"User '{request.UserId}' is not an assigned approver for this ADR.");
        }

        if (!string.Equals(approver.Status, "pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("DUPLICATE_DECISION",
                $"User '{request.UserId}' has already recorded a decision for this ADR.");
        }

        var decision = request.Decision.ToLowerInvariant();
        if (decision is not ("approved" or "rejected"))
        {
            throw new DomainException("INVALID_DECISION",
                $"Decision must be 'approved' or 'rejected'. Received: '{request.Decision}'.");
        }

        approver.Status = decision;
        approver.Comment = request.Comment;
        approver.DecidedAt = DateTime.UtcNow;

        // Evaluate threshold for auto-transition
        var config = await _configRepository.GetByKeyAsync("tenant_config", request.TenantSlug);
        var threshold = config?.ApprovalThreshold > 0 ? config.ApprovalThreshold : 1;

        var approvedCount = adr.Approvers.Count(a =>
            string.Equals(a.Status, "approved", StringComparison.OrdinalIgnoreCase));
        var hasRejections = adr.Approvers.Any(a =>
            string.Equals(a.Status, "rejected", StringComparison.OrdinalIgnoreCase));

        if (hasRejections)
        {
            adr.Status = "rejected";
        }
        else if (approvedCount >= threshold)
        {
            adr.Status = "approved";
        }

        adr.UpdatedAt = DateTime.UtcNow;
        await _adrRepository.UpdateAsync(adr, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "adr.decision_recorded",
            EntityType = "Adr",
            EntityId = adr.Id,
            UserId = request.UserId,
            Timestamp = DateTime.UtcNow,
            Details = $"Decision '{decision}' recorded for ADR '{adr.Id}'."
                + (request.Comment is not null ? $" Comment: {request.Comment}" : "")
        });

        return RecordDecisionResponse.FromEntity(adr);
    }
}
