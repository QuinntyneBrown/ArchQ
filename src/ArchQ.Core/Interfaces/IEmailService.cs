namespace ArchQ.Core.Interfaces;

public interface IEmailService
{
    Task SendVerificationEmailAsync(string email, string fullName, string token);
}
