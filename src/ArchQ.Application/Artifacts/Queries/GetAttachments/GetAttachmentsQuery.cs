using ArchQ.Application.Artifacts.Commands.UploadAttachment;
using MediatR;

namespace ArchQ.Application.Artifacts.Queries.GetAttachments;

public class GetAttachmentsQuery : IRequest<GetAttachmentsResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string AdrId { get; set; } = string.Empty;
}

public class GetAttachmentsResponse
{
    public List<AttachmentResponse> Items { get; set; } = new();
}
