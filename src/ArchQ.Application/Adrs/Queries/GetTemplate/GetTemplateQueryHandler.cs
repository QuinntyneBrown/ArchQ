using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetTemplate;

public class GetTemplateQueryHandler : IRequestHandler<GetTemplateQuery, TemplateResponse>
{
    private readonly IConfigRepository _configRepository;

    private const string DefaultTemplateBody = """
        ## Status

        Draft

        ## Context

        [Describe the context and problem statement...]

        ## Decision

        [Describe the decision that was made...]

        ## Consequences

        [List the consequences of the decision...]

        ## Participants

        - [Name]
        """;

    private static readonly List<string> DefaultRequiredSections = new()
    {
        "Status", "Context", "Decision", "Consequences"
    };

    public GetTemplateQueryHandler(IConfigRepository configRepository)
    {
        _configRepository = configRepository;
    }

    public async Task<TemplateResponse> Handle(GetTemplateQuery request, CancellationToken cancellationToken)
    {
        var config = await _configRepository.GetByKeyAsync("adr_template", request.TenantSlug);

        if (config is not null)
        {
            return new TemplateResponse
            {
                Body = config.Body,
                RequiredSections = config.RequiredSections,
                IsCustom = true,
                UpdatedAt = config.UpdatedAt
            };
        }

        return new TemplateResponse
        {
            Body = DefaultTemplateBody,
            RequiredSections = DefaultRequiredSections,
            IsCustom = false,
            UpdatedAt = null
        };
    }
}
