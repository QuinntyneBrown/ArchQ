using ArchQ.Core.Validation;

namespace ArchQ.Core.Exceptions;

public class ValidationFailedException : DomainException
{
    public List<ValidationError> Details { get; }

    public ValidationFailedException(string code, string message)
        : base(code, message)
    {
        Details = new List<ValidationError>();
    }

    public ValidationFailedException(string code, string message, List<ValidationError> details)
        : base(code, message)
    {
        Details = details;
    }
}
