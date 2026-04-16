using MediatR;

namespace ArchQ.Application.Tags.Queries.SuggestTags;

public class SuggestTagsQuery : IRequest<SuggestTagsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Query { get; set; } = string.Empty;
}

public class SuggestTagsResponse
{
    public List<TagSuggestion> Items { get; set; } = new();
}

public class TagSuggestion
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
}
