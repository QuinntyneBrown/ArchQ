namespace ArchQ.Core.Exceptions;

public class ConflictException : DomainException
{
    public ConflictException(string code, string message)
        : base(code, message)
    {
    }
}
