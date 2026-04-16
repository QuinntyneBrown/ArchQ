using ArchQ.Application.Adrs.Queries.GetTemplate;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.UpdateTemplate;

public class UpdateTemplateCommand : IRequest<TemplateResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> RequiredSections { get; set; } = new();
    public string ActorId { get; set; } = string.Empty;
}
