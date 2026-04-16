using ArchQ.Application.Artifacts.Commands.AddGeneralNote;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetGeneralNotes;

public class GetGeneralNotesQueryHandler : IRequestHandler<GetGeneralNotesQuery, GetGeneralNotesResponse>
{
    private readonly IGeneralNoteRepository _repository;

    public GetGeneralNotesQueryHandler(IGeneralNoteRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetGeneralNotesResponse> Handle(GetGeneralNotesQuery request, CancellationToken cancellationToken)
    {
        var notes = await _repository.ListByAdrAsync(request.AdrId, request.TenantSlug);

        return new GetGeneralNotesResponse
        {
            Items = notes.Select(n => new GeneralNoteResponse
            {
                Id = n.Id,
                AdrId = n.AdrId,
                Title = n.Title,
                Body = n.Body,
                AuthorId = n.AuthorId,
                CreatedAt = n.CreatedAt,
                UpdatedAt = n.UpdatedAt
            }).ToList()
        };
    }
}
