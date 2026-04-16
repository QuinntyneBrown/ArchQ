using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Config.Commands.UpdateApprovalThreshold;

public class UpdateApprovalThresholdCommandHandler : IRequestHandler<UpdateApprovalThresholdCommand, UpdateApprovalThresholdResponse>
{
    private readonly IConfigRepository _configRepository;

    public UpdateApprovalThresholdCommandHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<UpdateApprovalThresholdResponse> Handle(UpdateApprovalThresholdCommand request, CancellationToken cancellationToken)
    {
        var existing = await _configRepository.GetByKeyAsync("tenant_config", request.TenantSlug);

        var config = existing ?? new TemplateConfig();
        config.Key = "tenant_config";
        config.ApprovalThreshold = request.ApprovalThreshold;
        config.UpdatedAt = DateTime.UtcNow;
        config.UpdatedBy = request.ActorId;

        await _configRepository.UpsertAsync(config, request.TenantSlug);

        return new UpdateApprovalThresholdResponse
        {
            ApprovalThreshold = config.ApprovalThreshold,
            UpdatedAt = config.UpdatedAt
        };
    }
}
