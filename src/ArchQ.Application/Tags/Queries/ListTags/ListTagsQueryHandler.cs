using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tags.Queries.ListTags;

public class ListTagsQueryHandler : IRequestHandler<ListTagsQuery, ListTagsResponse>
{
    private readonly ITagRepository _tagRepository;

    public ListTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<ListTagsResponse> Handle(ListTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.ListAllAsync(request.TenantSlug);

        return new ListTagsResponse
        {
            Items = tags.Select(t => new TagItem
            {
                Name = t.Name,
                Slug = t.Slug,
                UsageCount = t.UsageCount,
                CreatedAt = t.CreatedAt
            }).ToList()
        };
    }
}
