namespace ArchQ.Application.Orgs.Commands.SwitchOrg;

public class SwitchOrgResponse
{
    public SwitchOrgTenantDto Tenant { get; set; } = new();
    public SwitchOrgUserDto User { get; set; } = new();
    public string AccessToken { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
}

public class SwitchOrgTenantDto
{
    public string Id { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}

public class SwitchOrgUserDto
{
    public string Id { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
}
