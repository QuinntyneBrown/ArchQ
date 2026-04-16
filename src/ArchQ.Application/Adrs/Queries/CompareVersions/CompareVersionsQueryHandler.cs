using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.CompareVersions;

public class CompareVersionsQueryHandler : IRequestHandler<CompareVersionsQuery, CompareVersionsResponse>
{
    private readonly IAdrVersionRepository _versionRepository;

    public CompareVersionsQueryHandler(IAdrVersionRepository versionRepository)
    {
        _versionRepository = versionRepository;
    }

    public async Task<CompareVersionsResponse> Handle(CompareVersionsQuery request, CancellationToken cancellationToken)
    {
        var fromVersion = await _versionRepository.GetByVersionAsync(request.AdrId, request.FromVersion, request.TenantSlug)
            ?? throw new NotFoundException("VERSION_NOT_FOUND", $"Version {request.FromVersion} not found.");

        var toVersion = await _versionRepository.GetByVersionAsync(request.AdrId, request.ToVersion, request.TenantSlug)
            ?? throw new NotFoundException("VERSION_NOT_FOUND", $"Version {request.ToVersion} not found.");

        return new CompareVersionsResponse
        {
            FromVersion = request.FromVersion,
            ToVersion = request.ToVersion,
            TitleDiff = ComputeLineDiff(fromVersion.Title, toVersion.Title),
            BodyDiff = ComputeLineDiff(fromVersion.Body, toVersion.Body)
        };
    }

    private static List<DiffLine> ComputeLineDiff(string fromText, string toText)
    {
        var fromLines = (fromText ?? string.Empty).Split('\n');
        var toLines = (toText ?? string.Empty).Split('\n');
        var diff = new List<DiffLine>();

        var maxLen = Math.Max(fromLines.Length, toLines.Length);

        for (int i = 0; i < maxLen; i++)
        {
            var fromLine = i < fromLines.Length ? fromLines[i] : null;
            var toLine = i < toLines.Length ? toLines[i] : null;

            if (fromLine == toLine)
            {
                diff.Add(new DiffLine { Type = "unchanged", Content = fromLine ?? string.Empty });
            }
            else
            {
                if (fromLine != null)
                {
                    diff.Add(new DiffLine { Type = "removed", Content = fromLine });
                }
                if (toLine != null)
                {
                    diff.Add(new DiffLine { Type = "added", Content = toLine });
                }
            }
        }

        return diff;
    }
}
