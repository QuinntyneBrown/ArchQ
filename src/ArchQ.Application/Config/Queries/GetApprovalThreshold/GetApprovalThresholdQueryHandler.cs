using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Config.Queries.GetApprovalThreshold;

public class GetApprovalThresholdQueryHandler : IRequestHandler<GetApprovalThresholdQuery, ApprovalThresholdResponse>
{
    private readonly IConfigRepository _configRepository;

    public GetApprovalThresholdQueryHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<ApprovalThresholdResponse> Handle(GetApprovalThresholdQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepository.GetByKeyAsync("tenant_config", request.TenantSlug);
        var threshold = config?.ApprovalThreshold > 0 ? config.ApprovalThreshold : 1;

        return new ApprovalThresholdResponse
        {
            ApprovalThreshold = threshold
        };
    }
}
