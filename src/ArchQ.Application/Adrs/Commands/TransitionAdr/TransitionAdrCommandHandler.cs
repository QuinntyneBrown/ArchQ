using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using ArchQ.Core.Validation;
using ArchQ.Core.Workflow;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.TransitionAdr;

public class TransitionAdrCommandHandler : IRequestHandler<TransitionAdrCommand, TransitionAdrResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IAuditRepository _auditRepository;
    private readonly IUserRepository _userRepository;

    public TransitionAdrCommandHandler(IAdrRepository adrRepository, IAuditRepository auditRepository, IUserRepository userRepository)
    {
        _adrRepository = adrRepository;
        _auditRepository = auditRepository;
        _userRepository = userRepository;
    }

    public async Task<TransitionAdrResponse> Handle(TransitionAdrCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new NotFoundException("NOT_FOUND", $"ADR '{request.AdrId}' not found.");

        var currentStatus = adr.Status;
        var targetStatus = request.TargetStatus;

        if (!WorkflowStateMachine.CanTransition(currentStatus, targetStatus))
        {
            throw new DomainException("INVALID_TRANSITION",
                $"Cannot transition from {currentStatus} to {targetStatus}.");
        }

        // Validate preconditions per transition
        switch (currentStatus, targetStatus)
        {
            case ("draft", "in-review"):
                var approverIds = request.ApproverIds ?? new List<string>();
                var validationErrors = await ApproverAssignmentValidator.ValidateAsync(
                    request.ActorId, approverIds, _userRepository, request.TenantSlug);

                if (validationErrors.Count > 0)
                {
                    throw new ValidationFailedException("VALIDATION_FAILED",
                        "Approver assignment validation failed.", validationErrors);
                }

                adr.Approvers = approverIds.Select(id => new AdrApprover
                {
                    UserId = id,
                    Status = "pending"
                }).ToList();
                break;

            case ("approved", "superseded"):
                if (string.IsNullOrEmpty(request.SupersededBy))
                {
                    throw new ValidationFailedException("VALIDATION_FAILED",
                        "SupersededBy must be provided when superseding an ADR.");
                }
                adr.SupersededBy = request.SupersededBy;
                break;

            case ("approved", "deprecated"):
                if (!request.ActorRoles.Contains("admin"))
                {
                    throw new ForbiddenException("FORBIDDEN",
                        "Only admins can deprecate an ADR.");
                }
                break;

            case ("rejected", "draft"):
                // Clear approver decisions
                foreach (var approver in adr.Approvers)
                {
                    approver.Status = "cleared";
                    approver.Comment = null;
                    approver.DecidedAt = null;
                }
                break;
        }

        adr.Status = targetStatus;
        adr.UpdatedAt = DateTime.UtcNow;

        await _adrRepository.UpdateAsync(adr, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "adr.transitioned",
            EntityType = "Adr",
            EntityId = adr.Id,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"ADR '{adr.AdrNumber}' transitioned from {currentStatus} to {targetStatus}."
                + (request.Reason is not null ? $" Reason: {request.Reason}" : "")
        });

        return TransitionAdrResponse.FromEntity(adr);
    }
}
