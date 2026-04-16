using ArchQ.Core.Interfaces;

namespace ArchQ.Core.Validation;

public static class ApproverAssignmentValidator
{
    public static async Task<List<ValidationError>> ValidateAsync(
        string authorId,
        List<string> approverIds,
        IUserRepository userRepo,
        string tenantSlug)
    {
        var errors = new List<ValidationError>();

        // At least 1 approver
        if (approverIds.Count == 0)
        {
            errors.Add(new ValidationError("approverIds", "APPROVERS_REQUIRED",
                "At least one approver is required."));
            return errors;
        }

        // No self-assignment
        if (approverIds.Contains(authorId))
        {
            errors.Add(new ValidationError("approverIds", "SELF_ASSIGNMENT",
                "The author cannot be assigned as an approver."));
        }

        // No duplicates
        var seen = new HashSet<string>();
        foreach (var id in approverIds)
        {
            if (!seen.Add(id))
            {
                errors.Add(new ValidationError("approverIds", "DUPLICATE_APPROVER",
                    $"Duplicate approver: '{id}'."));
            }
        }

        // Look up all users in one call
        var distinctIds = approverIds.Distinct().ToList();
        var users = await userRepo.GetByIdsAsync(distinctIds, tenantSlug);

        foreach (var id in distinctIds)
        {
            if (!users.TryGetValue(id, out var user))
            {
                errors.Add(new ValidationError("approverIds", "USER_NOT_FOUND",
                    $"User '{id}' not found in tenant '{tenantSlug}'."));
                continue;
            }

            // Must have "reviewer" or "admin" role
            if (!user.Roles.Contains("reviewer") && !user.Roles.Contains("admin"))
            {
                errors.Add(new ValidationError("approverIds", "INVALID_ROLE",
                    $"User '{id}' does not have 'reviewer' or 'admin' role."));
            }
        }

        return errors;
    }
}
