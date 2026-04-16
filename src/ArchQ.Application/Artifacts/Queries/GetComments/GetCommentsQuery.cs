using ArchQ.Application.Artifacts.Commands.AddComment;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetComments;

public class GetCommentsQuery : IRequest<GetCommentsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}

public class GetCommentsResponse
{
    public List<CommentResponse> Items { get; set; } = new();
}
