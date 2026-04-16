using MediatR;

namespace ArchQ.Application.Adrs.Commands.RecordDecision;

public class RecordDecisionCommand : IRequest<RecordDecisionResponse>
{
    public string AdrId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Decision { get; set; } = string.Empty;
    public string? Comment { get; set; }
}
