using MediatR;

namespace ArchQ.Application.Adrs.Commands.UpdateAdr;

public class UpdateAdrCommand : IRequest<UpdateAdrResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string ActorId { get; set; } = string.Empty;
    public List<string> ActorRoles { get; set; } = new();
}
