using MediatR;

namespace ArchQ.Application.Tags.Queries.ListTags;

public class ListTagsQuery : IRequest<ListTagsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
}

public class ListTagsResponse
{
    public List<TagItem> Items { get; set; } = new();
}

public class TagItem
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public int UsageCount { get; set; }
    public DateTime CreatedAt { get; set; }
}
