using ArchQ.Application.Auth.Commands.Login;

namespace ArchQ.Application.Auth.Commands.RefreshToken;

public class RefreshTokenResponse
{
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public LoginUserDto User { get; set; } = new();
    public LoginTenantDto Tenant { get; set; } = new();
    public List<LoginMembershipDto> Memberships { get; set; } = new();
}
