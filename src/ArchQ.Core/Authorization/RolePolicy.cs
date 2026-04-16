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
                "user.manage",
                "role.assign",
                "adr.create",
                "adr.edit.own",
                "adr.edit.any",
                "adr.delete",
                "adr.submit_review",
                "adr.approve",
                "adr.reject",
                "adr.view",
                "comment.create",
                "comment.edit.own",
                "comment.delete.own",
                "comment.delete.any",
                "note.create",
                "note.view",
                "attachment.upload",
                "attachment.view",
                "audit.view",
                "config.manage"
            },
            ["author"] = new[]
            {
                "adr.create",
                "adr.edit.own",
                "adr.submit_review",
                "adr.view",
                "comment.create",
                "comment.edit.own",
                "comment.delete.own",
                "note.create",
                "note.view",
                "attachment.upload",
                "attachment.view"
            },
            ["reviewer"] = new[]
            {
                "adr.approve",
                "adr.reject",
                "adr.view",
                "comment.create",
                "comment.edit.own",
                "comment.delete.own",
                "note.view",
                "attachment.view"
            },
            ["viewer"] = new[]
            {
                "adr.view",
                "note.view",
                "attachment.view"
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
