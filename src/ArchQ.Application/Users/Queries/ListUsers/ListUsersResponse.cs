namespace ArchQ.Application.Users.Queries.ListUsers;

public class ListUsersResponse
{
    public List<ListUsersItem> Items { get; set; } = new();
}

public class ListUsersItem
{
    public string Id { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}
