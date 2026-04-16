using ArchQ.Application.Artifacts.Commands.AddComment;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.EditComment;

public class EditCommentCommand : IRequest<CommentResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string CommentId { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string ActorId { get; set; } = string.Empty;
}
