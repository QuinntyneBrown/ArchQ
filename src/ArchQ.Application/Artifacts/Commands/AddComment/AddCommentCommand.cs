using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddComment;

public class AddCommentCommand : IRequest<CommentResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
}

public class CommentResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string? ParentCommentId { get; set; }
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public bool Edited { get; set; }
    public bool Deleted { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public List<CommentResponse> Replies { get; set; } = new();
}
