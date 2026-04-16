namespace ArchQ.Application.Orgs.Queries.ListMemberships;

public class ListMembershipsResponse
{
    public List<MembershipDto> Memberships { get; set; } = new();
}

public class MembershipDto
{
    public string TenantId { get; set; } = string.Empty;
    public string TenantSlug { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public List<string> Roles { get; set; } = new();
    public bool IsActive { get; set; }
}
