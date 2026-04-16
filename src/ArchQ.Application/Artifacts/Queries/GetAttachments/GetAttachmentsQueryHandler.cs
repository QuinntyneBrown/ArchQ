using ArchQ.Application.Artifacts.Commands.UploadAttachment;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetAttachments;

public class GetAttachmentsQueryHandler : IRequestHandler<GetAttachmentsQuery, GetAttachmentsResponse>
{
    private readonly IAttachmentRepository _repository;

    public GetAttachmentsQueryHandler(IAttachmentRepository repository)
    {
        _repository = repository;
    }

    public async Task<GetAttachmentsResponse> Handle(GetAttachmentsQuery request, CancellationToken cancellationToken)
    {
        var attachments = await _repository.ListByAdrAsync(request.AdrId, request.TenantSlug);

        return new GetAttachmentsResponse
        {
            Items = attachments.Select(a => new AttachmentResponse
            {
                Id = a.Id,
                AdrId = a.AdrId,
                FileName = a.FileName,
                DisplayName = a.DisplayName,
                Description = a.Description,
                ContentType = a.ContentType,
                FileSize = a.FileSize,
                AuthorId = a.AuthorId,
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            }).ToList()
        };
    }
}
