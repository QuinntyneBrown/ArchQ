using ArchQ.Core.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ArchQ.Infrastructure.Email;

public class SmtpEmailService : IEmailService
{
    private readonly ILogger<SmtpEmailService> _logger;
    private readonly IHostEnvironment _hostEnvironment;

    // In-memory store for test verification tokens (Development only)
    // Shared with AuthController via static access
    public static readonly Dictionary<string, string> TestVerificationTokens = new();

    public SmtpEmailService(ILogger<SmtpEmailService> logger, IHostEnvironment hostEnvironment)
    {
        _logger = logger;
        _hostEnvironment = hostEnvironment;
    }

    public Task SendVerificationEmailAsync(string email, string fullName, string token)
    {
        var verificationUrl = $"http://localhost:4200/auth/verify-email?token={Uri.EscapeDataString(token)}";

        _logger.LogInformation(
            "Verification email for {Email} ({FullName}): {Url}",
            email, fullName, verificationUrl);

        if (_hostEnvironment.IsDevelopment())
        {
            TestVerificationTokens[email.ToLowerInvariant()] = token;
        }

        return Task.CompletedTask;
    }
}
