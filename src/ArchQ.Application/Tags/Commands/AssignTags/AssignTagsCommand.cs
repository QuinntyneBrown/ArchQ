using MediatR;

namespace ArchQ.Application.Tags.Commands.AssignTags;

public class AssignTagsCommand : IRequest<AssignTagsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string ActorId { get; set; } = string.Empty;
}

public class AssignTagsResponse
{
    public List<string> Tags { get; set; } = new();
}
