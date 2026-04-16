namespace ArchQ.Core.Authorization;

public static class RolePolicy
{
    public static readonly IReadOnlyList<string> ValidRoles = new[]
    {
        "admin", "author", "reviewer", "viewer"
    };

    public static readonly IReadOnlyDictionary<string, IReadOnlyList<string>> Permissions =
        new Dictionary<string, IReadOnlyList<string>>
        {
            ["admin"] = new[]
            {
                "tenant.manage",
                "user.invite",
                "user.remove",
                "role.assign",
                "role.revoke",
                "adr.create",
                "adr.edit",
                "adr.delete",
                "adr.approve",
                "adr.comment",
                "adr.view"
            },
            ["author"] = new[]
            {
                "adr.create",
                "adr.edit",
                "adr.comment",
                "adr.view"
            },
            ["reviewer"] = new[]
            {
                "adr.approve",
                "adr.comment",
                "adr.view"
            },
            ["viewer"] = new[]
            {
                "adr.view"
            }
        };

    public static bool IsValidRole(string role)
    {
        return ValidRoles.Contains(role.ToLowerInvariant());
    }

    public static bool CanPerform(List<string> roles, string permission)
    {
        return roles.Any(role =>
            Permissions.TryGetValue(role.ToLowerInvariant(), out var perms)
            && perms.Contains(permission));
    }

    public static List<string> GetEffectivePermissions(List<string> roles)
    {
        return roles
            .Where(r => Permissions.ContainsKey(r.ToLowerInvariant()))
            .SelectMany(r => Permissions[r.ToLowerInvariant()])
            .Distinct()
            .ToList();
    }

    public static bool CanApproveAdr(string userId, string adrAuthorId, List<string> roles)
    {
        if (string.Equals(userId, adrAuthorId, StringComparison.Ordinal))
        {
            return false;
        }

        return CanPerform(roles, "adr.approve");
    }
}
