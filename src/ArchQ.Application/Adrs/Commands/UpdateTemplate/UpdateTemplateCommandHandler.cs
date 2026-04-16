using ArchQ.Application.Adrs.Queries.GetTemplate;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.UpdateTemplate;

public class UpdateTemplateCommandHandler : IRequestHandler<UpdateTemplateCommand, TemplateResponse>
{
    private readonly IConfigRepository _configRepository;

    public UpdateTemplateCommandHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<TemplateResponse> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
    {
        var existing = await _configRepository.GetByKeyAsync("adr_template", request.TenantSlug);

        var config = existing ?? new TemplateConfig();
        config.Key = "adr_template";
        config.Body = request.Body;
        config.RequiredSections = request.RequiredSections;
        config.UpdatedAt = DateTime.UtcNow;
        config.UpdatedBy = request.ActorId;

        await _configRepository.UpsertAsync(config, request.TenantSlug);

        return new TemplateResponse
        {
            Body = config.Body,
            RequiredSections = config.RequiredSections,
            IsCustom = true,
            UpdatedAt = config.UpdatedAt
        };
    }
}
