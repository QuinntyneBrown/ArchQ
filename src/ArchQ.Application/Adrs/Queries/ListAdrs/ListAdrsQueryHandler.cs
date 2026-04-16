using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Adrs.Queries.ListAdrs;

public class ListAdrsQueryHandler : IRequestHandler<ListAdrsQuery, AdrListResponse>
{
    private readonly IAdrRepository _adrRepository;
    private readonly IUserRepository _userRepository;

    public ListAdrsQueryHandler(IAdrRepository adrRepository, IUserRepository userRepository)
    {
        _adrRepository = adrRepository;
        _userRepository = userRepository;
    }

    public async Task<AdrListResponse> Handle(ListAdrsQuery request, CancellationToken cancellationToken)
    {
        var pageSize = Math.Clamp(request.PageSize, 1, 100);

        var listParams = new AdrListParams
        {
            Status = request.Status,
            AuthorId = request.AuthorId,
            Tags = request.Tags,
            DateFrom = request.DateFrom,
            DateTo = request.DateTo,
            SortField = request.SortField,
            SortDirection = request.SortDirection,
            Cursor = request.Cursor,
            PageSize = pageSize
        };

        var listResult = await _adrRepository.ListAsync(listParams, request.TenantSlug);
        var totalCount = await _adrRepository.CountAsync(listParams, request.TenantSlug);

        // Batch resolve author names
        var authorIds = listResult.Items
            .Select(i => i.AuthorId)
            .Where(id => !string.IsNullOrEmpty(id))
            .Distinct()
            .ToList();

        var authors = authorIds.Count > 0
            ? await _userRepository.GetByIdsAsync(authorIds, request.TenantSlug)
            : new Dictionary<string, User>();

        var items = listResult.Items.Select(item =>
        {
            var authorName = authors.TryGetValue(item.AuthorId, out var user)
                ? user.FullName
                : string.Empty;

            return new AdrListItemResponse
            {
                Id = item.Id,
                AdrNumber = item.AdrNumber,
                Title = item.Title,
                Status = item.Status,
                AuthorId = item.AuthorId,
                AuthorName = authorName,
                Tags = item.Tags,
                UpdatedAt = item.UpdatedAt
            };
        }).ToList();

        return new AdrListResponse
        {
            Items = items,
            NextCursor = listResult.NextCursor,
            PrevCursor = listResult.PrevCursor,
            TotalCount = totalCount
        };
    }
}
