using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tags.Queries.SuggestTags;

public class SuggestTagsQueryHandler : IRequestHandler<SuggestTagsQuery, SuggestTagsResponse>
{
    private readonly ITagRepository _tagRepository;

    public SuggestTagsQueryHandler(ITagRepository tagRepository)
    {
        _tagRepository = tagRepository;
    }

    public async Task<SuggestTagsResponse> Handle(SuggestTagsQuery request, CancellationToken cancellationToken)
    {
        var tags = await _tagRepository.SearchAsync(request.Query, request.TenantSlug);

        return new SuggestTagsResponse
        {
            Items = tags.Select(t => new TagSuggestion
            {
                Name = t.Name,
                Slug = t.Slug,
                UsageCount = t.UsageCount
            }).ToList()
        };
    }
}
