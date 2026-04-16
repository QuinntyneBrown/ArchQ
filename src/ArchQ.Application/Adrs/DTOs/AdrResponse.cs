using ArchQ.Core.Entities;

namespace ArchQ.Application.Adrs.DTOs;

public class AdrResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public int Version { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public static AdrResponse FromEntity(Adr adr) => new()
    {
        Id = adr.Id,
        AdrNumber = adr.AdrNumber,
        Title = adr.Title,
        Status = adr.Status,
        AuthorId = adr.AuthorId,
        Tags = adr.Tags,
        Version = adr.Version,
        CreatedAt = adr.CreatedAt,
        UpdatedAt = adr.UpdatedAt
    };
}
