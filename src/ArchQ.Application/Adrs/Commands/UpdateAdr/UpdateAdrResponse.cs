namespace ArchQ.Application.Adrs.Commands.UpdateAdr;

public class UpdateAdrResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrNumber { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Version { get; set; }
    public DateTime UpdatedAt { get; set; }
}
