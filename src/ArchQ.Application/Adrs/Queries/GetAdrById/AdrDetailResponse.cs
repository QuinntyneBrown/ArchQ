using ArchQ.Core.Entities;

namespace ArchQ.Application.Adrs.Queries.GetAdrById;

public class AdrDetailResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static AdrDetailResponse FromEntity(Adr adr) => new()
    {
        Id = adr.Id,
        AdrNumber = adr.AdrNumber,
        Title = adr.Title,
        Body = adr.Body,
        Status = adr.Status,
        AuthorId = adr.AuthorId,
        Tags = adr.Tags,
        Version = adr.Version,
        CreatedAt = adr.CreatedAt,
        UpdatedAt = adr.UpdatedAt
    };
}
