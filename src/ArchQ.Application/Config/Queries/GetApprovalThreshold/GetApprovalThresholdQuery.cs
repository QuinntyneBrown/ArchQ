using MediatR;

namespace ArchQ.Application.Config.Queries.GetApprovalThreshold;

public class GetApprovalThresholdQuery : IRequest<ApprovalThresholdResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
}
