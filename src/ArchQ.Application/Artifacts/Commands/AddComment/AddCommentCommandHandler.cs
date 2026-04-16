using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddComment;

public class AddCommentCommandHandler : IRequestHandler<AddCommentCommand, CommentResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IAuditRepository _auditRepository;

    public AddCommentCommandHandler(
        IAdrRepository adrRepository,
        ICommentRepository commentRepository,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _commentRepository = commentRepository;
        _auditRepository = auditRepository;
    }

    public async Task<CommentResponse> Handle(AddCommentCommand request, CancellationToken cancellationToken)
    {
        // Verify ADR exists (no status restriction for comments)
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new InvalidOperationException($"ADR '{request.AdrId}' not found.");

        // If replying to a parent comment, verify it exists
        if (!string.IsNullOrEmpty(request.ParentCommentId))
        {
            var parent = await _commentRepository.GetByIdAsync(request.ParentCommentId, request.TenantSlug)
                ?? throw new InvalidOperationException($"Parent comment '{request.ParentCommentId}' not found.");

            if (parent.Deleted)
            {
                throw new InvalidOperationException("Cannot reply to a deleted comment.");
            }
        }

        var comment = new Comment
        {
            AdrId = request.AdrId,
            ParentCommentId = request.ParentCommentId,
            Body = request.Body,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var created = await _commentRepository.CreateAsync(comment, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "comment.created",
            EntityType = "Comment",
            EntityId = created.Id,
            UserId = request.AuthorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Comment added to ADR '{adr.AdrNumber}'."
        });

        return new CommentResponse
        {
            Id = created.Id,
            AdrId = created.AdrId,
            ParentCommentId = created.ParentCommentId,
            Body = created.Body,
            AuthorId = created.AuthorId,
            Edited = created.Edited,
            Deleted = created.Deleted,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
