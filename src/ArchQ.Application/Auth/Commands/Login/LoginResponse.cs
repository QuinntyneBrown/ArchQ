namespace ArchQ.Application.Auth.Commands.Login;

public class LoginResponse
{
    public LoginUserDto User { get; set; } = new();
    public LoginTenantDto Tenant { get; set; } = new();
    public List<LoginMembershipDto> Memberships { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class LoginUserDto
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}

public class LoginTenantDto
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class LoginMembershipDto
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
