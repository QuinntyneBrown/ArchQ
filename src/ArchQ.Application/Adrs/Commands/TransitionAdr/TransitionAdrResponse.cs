using ArchQ.Core.Entities;

namespace ArchQ.Application.Adrs.Commands.TransitionAdr;

public class TransitionAdrResponse
{
    public string Id { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public List<AdrApproverDto> Approvers { get; set; } = new();
    public DateTime UpdatedAt { get; set; }

    public static TransitionAdrResponse FromEntity(Adr adr) => new()
    {
        Id = adr.Id,
        Status = adr.Status,
        Approvers = adr.Approvers.Select(a => new AdrApproverDto
        {
            UserId = a.UserId,
            Status = a.Status,
            Comment = a.Comment,
            DecidedAt = a.DecidedAt
        }).ToList(),
        UpdatedAt = adr.UpdatedAt
    };
}

public class AdrApproverDto
{
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? Comment { get; set; }
    public DateTime? DecidedAt { get; set; }
}
