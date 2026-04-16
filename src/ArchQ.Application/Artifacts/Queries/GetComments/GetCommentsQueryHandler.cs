using ArchQ.Application.Artifacts.Commands.AddComment;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetComments;

public class GetCommentsQueryHandler : IRequestHandler<GetCommentsQuery, GetCommentsResponse>
{
    private readonly ICommentRepository _repository;

    public GetCommentsQueryHandler(ICommentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetCommentsResponse> Handle(GetCommentsQuery request, CancellationToken cancellationToken)
    {
        var allComments = await _repository.ListByAdrAsync(request.AdrId, request.TenantSlug);

        // Build threaded structure: top-level comments with nested replies
        var commentMap = allComments.ToDictionary(c => c.Id);
        var topLevel = new List<CommentResponse>();

        var responseMap = allComments.Select(c => new CommentResponse
        {
            Id = c.Id,
            AdrId = c.AdrId,
            ParentCommentId = c.ParentCommentId,
            Body = c.Deleted ? "[deleted]" : c.Body,
            AuthorId = c.AuthorId,
            Edited = c.Edited,
            Deleted = c.Deleted,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        }).ToDictionary(r => r.Id);

        foreach (var response in responseMap.Values)
        {
            if (string.IsNullOrEmpty(response.ParentCommentId))
            {
                topLevel.Add(response);
            }
            else if (responseMap.TryGetValue(response.ParentCommentId, out var parent))
            {
                parent.Replies.Add(response);
            }
            else
            {
                // Orphan reply — treat as top-level
                topLevel.Add(response);
            }
        }

        return new GetCommentsResponse
        {
            Items = topLevel.OrderBy(c => c.CreatedAt).ToList()
        };
    }
}
