using System.Text.RegularExpressions;
using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tags.Commands.AssignTags;

public class AssignTagsCommandHandler : IRequestHandler<AssignTagsCommand, AssignTagsResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly ITagRepository _tagRepository;
    private readonly IAuditRepository _auditRepository;

    public AssignTagsCommandHandler(
        IAdrRepository adrRepository,
        ITagRepository tagRepository,
        IAuditRepository auditRepository)
    {
        _adrRepository = adrRepository;
        _tagRepository = tagRepository;
        _auditRepository = auditRepository;
    }

    public async Task<AssignTagsResponse> Handle(AssignTagsCommand request, CancellationToken cancellationToken)
    {
        var adr = await _adrRepository.GetByIdAsync(request.AdrId, request.TenantSlug)
            ?? throw new NotFoundException("NOT_FOUND", "ADR not found.");

        var previousTags = new HashSet<string>(adr.Tags, StringComparer.OrdinalIgnoreCase);
        var newTags = request.Tags
            .Select(t => t.Trim().ToLowerInvariant())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Distinct()
            .ToList();
        var newTagSet = new HashSet<string>(newTags, StringComparer.OrdinalIgnoreCase);

        // Create any tags that don't exist yet and increment usage for newly added tags
        foreach (var tagName in newTags)
        {
            var slug = Slugify(tagName);
            var existing = await _tagRepository.GetBySlugAsync(slug, request.TenantSlug);
            if (existing == null)
            {
                await _tagRepository.CreateAsync(new Tag
                {
                    Id = Tag.GenerateId(slug),
                    Name = tagName,
                    Slug = slug,
                    UsageCount = 0,
                    TenantSlug = request.TenantSlug,
                    CreatedAt = DateTime.UtcNow
                }, request.TenantSlug);
            }

            if (!previousTags.Contains(tagName))
            {
                await _tagRepository.IncrementUsageAsync(slug, request.TenantSlug);
            }
        }

        // Decrement usage for removed tags
        foreach (var oldTag in previousTags)
        {
            if (!newTagSet.Contains(oldTag))
            {
                var slug = Slugify(oldTag);
                await _tagRepository.DecrementUsageAsync(slug, request.TenantSlug);
            }
        }

        adr.Tags = newTags;
        adr.UpdatedAt = DateTime.UtcNow;
        await _adrRepository.UpdateAsync(adr, request.TenantSlug);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = request.TenantSlug,
            Action = "adr.tags_assigned",
            EntityType = "Adr",
            EntityId = adr.Id,
            UserId = request.ActorId,
            Timestamp = DateTime.UtcNow,
            Details = $"Tags updated to: [{string.Join(", ", newTags)}]"
        });

        return new AssignTagsResponse { Tags = newTags };
    }

    private static string Slugify(string input)
    {
        var slug = input.Trim().ToLowerInvariant();
        slug = Regex.Replace(slug, @"[^a-z0-9\s-]", "");
        slug = Regex.Replace(slug, @"[\s]+", "-");
        slug = Regex.Replace(slug, @"-+", "-");
        return slug.Trim('-');
    }
}
