namespace ArchQ.Application.Roles.DTOs;

public class RoleChangeResponse
{
    public string UserId { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public DateTime UpdatedAt { get; set; }
}
