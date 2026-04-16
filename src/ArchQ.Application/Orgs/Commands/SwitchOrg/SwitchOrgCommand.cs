using MediatR;

namespace ArchQ.Application.Orgs.Commands.SwitchOrg;

public class SwitchOrgCommand : IRequest<SwitchOrgResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string CurrentRefreshToken { get; set; } = string.Empty;
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}
