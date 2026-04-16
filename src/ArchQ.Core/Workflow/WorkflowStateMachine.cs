namespace ArchQ.Core.Workflow;

public static class WorkflowStateMachine
{
    private static readonly Dictionary<string, List<string>> PermittedTransitions = new()
    {
        { "draft", new List<string> { "in-review" } },
        { "in-review", new List<string> { "approved", "rejected" } },
        { "approved", new List<string> { "superseded", "deprecated" } },
        { "rejected", new List<string> { "draft" } },
        { "superseded", new List<string>() },
        { "deprecated", new List<string>() }
    };

    public static readonly IReadOnlyList<string> ValidStatusValues =
        new List<string> { "draft", "in-review", "approved", "rejected", "superseded", "deprecated" }.AsReadOnly();

    public static bool CanTransition(string currentStatus, string targetStatus)
    {
        if (PermittedTransitions.TryGetValue(currentStatus, out var allowed))
        {
            return allowed.Contains(targetStatus);
        }

        return false;
    }
}
