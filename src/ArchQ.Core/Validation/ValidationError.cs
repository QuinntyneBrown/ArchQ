namespace ArchQ.Core.Validation;

public record ValidationError(string Field, string Rule, string Message);
