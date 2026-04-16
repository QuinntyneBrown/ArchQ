using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Artifacts.Commands.UploadAttachment;

public class UploadAttachmentCommandHandler : IRequestHandler<UploadAttachmentCommand, AttachmentResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IAttachmentRepository _attachmentRepository;
    private readonly IFileStorageService _fileStorageService;
    private readonly IAuditRepository _auditRepository;

    private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
    {
        ".pdf", ".png", ".jpg", ".jpeg", ".svg", ".drawio"
    };

    private const long MaxFileSize = 10 * 1024 * 1024; // 10 MB

    public UploadAttachmentCommandHandler(
        IAdrRepository adrRepository,
        IAttachmentRepository attachmentRepository,
        IFileStorageService fileStorageService,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _attachmentRepository = attachmentRepository;
        _fileStorageService = fileStorageService;
        _auditRepository = auditRepository;
    }

    public async Task<AttachmentResponse> Handle(UploadAttachmentCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new InvalidOperationException($"ADR '{request.AdrId}' not found.");

        if (adr.Status != "draft" && adr.Status != "in_review")
        {
            throw new InvalidOperationException("Attachments can only be added to ADRs in Draft or In Review status.");
        }

        var extension = Path.GetExtension(request.FileName);
        if (!AllowedExtensions.Contains(extension))
        {
            throw new InvalidOperationException($"File type '{extension}' is not allowed. Allowed types: pdf, png, jpg, svg, drawio.");
        }

        if (request.FileSize > MaxFileSize)
        {
            throw new InvalidOperationException($"File size exceeds the maximum allowed size of 10 MB.");
        }

        var attachment = new AttachmentMeta
        {
            AdrId = request.AdrId,
            FileName = request.FileName,
            DisplayName = request.DisplayName,
            Description = request.Description,
            ContentType = request.ContentType,
            FileSize = request.FileSize,
            AuthorId = request.AuthorId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // StorageKey uses tenant + ADR + attachment id for uniqueness
        attachment.StorageKey = $"{request.TenantSlug}/{request.AdrId}/{attachment.Id}/{request.FileName}";

        await _fileStorageService.UploadAsync(attachment.StorageKey, request.FileStream, request.ContentType);

        var created = await _attachmentRepository.CreateAsync(attachment, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "attachment.uploaded",
            EntityType = "AttachmentMeta",
            EntityId = created.Id,
            UserId = request.AuthorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Attachment '{created.DisplayName}' uploaded to ADR '{adr.AdrNumber}'."
        });

        return new AttachmentResponse
        {
            Id = created.Id,
            AdrId = created.AdrId,
            FileName = created.FileName,
            DisplayName = created.DisplayName,
            Description = created.Description,
            ContentType = created.ContentType,
            FileSize = created.FileSize,
            AuthorId = created.AuthorId,
            CreatedAt = created.CreatedAt,
            UpdatedAt = created.UpdatedAt
        };
    }
}
