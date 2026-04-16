using ArchQ.Application.Artifacts.Commands.AddComment;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.EditComment;

public class EditCommentCommandHandler : IRequestHandler<EditCommentCommand, CommentResponse>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IAuditRepository _auditRepository;
    private static readonly TimeSpan EditWindow = TimeSpan.FromMinutes(15);

    public EditCommentCommandHandler(ICommentRepository commentRepository, IAuditRepository auditRepository)
    {
        _commentRepository = commentRepository;
        _auditRepository = auditRepository;
    }

    public async Task<CommentResponse> Handle(EditCommentCommand request, CancellationToken cancellationToken)
    {
        var comment = await _commentRepository.GetByIdAsync(request.CommentId, request.TenantSlug)
            ?? throw new InvalidOperationException($"Comment '{request.CommentId}' not found.");

        if (comment.AuthorId != request.ActorId)
        {
            throw new InvalidOperationException("Only the comment author can edit a comment.");
        }

        if (comment.Deleted)
        {
            throw new InvalidOperationException("Cannot edit a deleted comment.");
        }

        if (DateTime.UtcNow - comment.CreatedAt > EditWindow)
        {
            throw new InvalidOperationException("Comments can only be edited within 15 minutes of creation.");
        }

        comment.Body = request.Body;
        comment.Edited = true;
        comment.UpdatedAt = DateTime.UtcNow;

        var updated = await _commentRepository.UpdateAsync(comment, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "comment.edited",
            EntityType = "Comment",
            EntityId = updated.Id,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Comment '{updated.Id}' edited."
        });

        return new CommentResponse
        {
            Id = updated.Id,
            AdrId = updated.AdrId,
            ParentCommentId = updated.ParentCommentId,
            Body = updated.Body,
            AuthorId = updated.AuthorId,
            Edited = updated.Edited,
            Deleted = updated.Deleted,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };
    }
}
