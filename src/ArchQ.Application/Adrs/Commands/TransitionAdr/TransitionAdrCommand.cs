using MediatR;

namespace ArchQ.Application.Adrs.Commands.TransitionAdr;

public class TransitionAdrCommand : IRequest<TransitionAdrResponse>
{
    public string AdrId { get; set; } = string.Empty;
    public string TargetStatus { get; set; } = string.Empty;
    public List<string>? ApproverIds { get; set; }
    public string? SupersededBy { get; set; }
    public string? Reason { get; set; }
    public string TenantSlug { get; set; } = string.Empty;
    public string ActorId { get; set; } = string.Empty;
    public List<string> ActorRoles { get; set; } = new();
}
