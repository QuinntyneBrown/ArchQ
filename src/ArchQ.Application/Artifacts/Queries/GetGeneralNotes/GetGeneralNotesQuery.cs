using ArchQ.Application.Artifacts.Commands.AddGeneralNote;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetGeneralNotes;

public class GetGeneralNotesQuery : IRequest<GetGeneralNotesResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}

public class GetGeneralNotesResponse
{
    public List<GeneralNoteResponse> Items { get; set; } = new();
}
