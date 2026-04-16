namespace ArchQ.Application.Roles.DTOs;

public class UserRolesResponse
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
}
