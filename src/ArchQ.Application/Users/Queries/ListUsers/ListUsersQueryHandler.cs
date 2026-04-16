using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Users.Queries.ListUsers;

public class ListUsersQueryHandler : IRequestHandler<ListUsersQuery, ListUsersResponse>
{
    private readonly IUserRepository _userRepository;

    public ListUsersQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<ListUsersResponse> Handle(ListUsersQuery request, CancellationToken cancellationToken)
    {
        var users = string.IsNullOrEmpty(request.Role)
            ? throw new ArgumentException("Role filter is required.")
            : await _userRepository.ListByRoleAsync(request.Role, request.TenantSlug);

        return new ListUsersResponse
        {
            Items = users.Select(u => new ListUsersItem
            {
                Id = u.Id,
                FullName = u.FullName,
                Email = u.Email
            }).ToList()
        };
    }
}
