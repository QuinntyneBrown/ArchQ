using MediatR;

namespace ArchQ.Application.Config.Commands.UpdateApprovalThreshold;

public class UpdateApprovalThresholdCommand : IRequest<UpdateApprovalThresholdResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public int ApprovalThreshold { get; set; }
    public string ActorId { get; set; } = string.Empty;
}
