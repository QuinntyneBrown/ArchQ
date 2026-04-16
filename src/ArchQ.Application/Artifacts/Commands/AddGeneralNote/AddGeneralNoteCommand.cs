using MediatR;

namespace ArchQ.Application.Artifacts.Commands.AddGeneralNote;

public class AddGeneralNoteCommand : IRequest<GeneralNoteResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
}

public class GeneralNoteResponse
{
    public string Id { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
