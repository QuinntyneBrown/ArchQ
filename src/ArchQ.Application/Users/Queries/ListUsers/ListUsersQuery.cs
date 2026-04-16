using MediatR;

namespace ArchQ.Application.Users.Queries.ListUsers;

public class ListUsersQuery : IRequest<ListUsersResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string? Role { get; set; }
}
