namespace ArchQ.Core.Authorization;

public record RoleDefinition(
    string Role,
    string DisplayName,
    string Description,
    List<string> Permissions);
