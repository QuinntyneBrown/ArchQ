using MediatR;

namespace ArchQ.Application.Adrs.Queries.GetTemplate;

public class GetTemplateQuery : IRequest<TemplateResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
}
