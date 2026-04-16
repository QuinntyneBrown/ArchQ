namespace ArchQ.Core.Exceptions;

public class ValidationFailedException : DomainException
{
    public ValidationFailedException(string code, string message)
        : base(code, message)
    {
    }
}
