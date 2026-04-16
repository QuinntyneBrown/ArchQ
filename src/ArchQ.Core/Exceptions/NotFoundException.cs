namespace ArchQ.Core.Exceptions;

public class NotFoundException : DomainException
{
    public NotFoundException(string code, string message)
        : base(code, message)
    {
    }
}
