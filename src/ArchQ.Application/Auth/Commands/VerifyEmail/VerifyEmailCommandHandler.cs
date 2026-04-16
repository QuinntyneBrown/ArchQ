using System.Security.Cryptography;
using System.Text;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Auth.Commands.VerifyEmail;

public class VerifyEmailCommandHandler : IRequestHandler<VerifyEmailCommand, VerifyEmailResponse>
{
    private readonly IVerificationTokenRepository _verificationTokenRepository;
    private readonly IUserRepository _userRepository;

    public VerifyEmailCommandHandler(
        IVerificationTokenRepository verificationTokenRepository,
        IUserRepository userRepository)
    {
        _verificationTokenRepository = verificationTokenRepository;
        _userRepository = userRepository;
    }

    public async Task<VerifyEmailResponse> Handle(VerifyEmailCommand request, CancellationToken cancellationToken)
    {
        var tokenHash = ComputeSha256(request.Token);

        var verificationToken = await _verificationTokenRepository.GetByTokenHashAsync(tokenHash);

        if (verificationToken is null || verificationToken.Used || verificationToken.ExpiresAt < DateTime.UtcNow)
        {
            throw new DomainException("INVALID_TOKEN", "Verification link is invalid or has expired.");
        }

        // Update user
        var user = await _userRepository.GetByIdAsync(verificationToken.UserId, verificationToken.TenantSlug);

        if (user is null)
        {
            throw new DomainException("INVALID_TOKEN", "Verification link is invalid or has expired.");
        }

        user.EmailVerified = true;
        user.Status = "active";
        user.UpdatedAt = DateTime.UtcNow;
        await _userRepository.UpdateAsync(user, verificationToken.TenantSlug);

        // Mark token as used
        verificationToken.Used = true;
        await _verificationTokenRepository.UpdateAsync(verificationToken);

        return new VerifyEmailResponse
        {
            Message = "Email verified successfully. You can now sign in."
        };
    }

    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexStringLower(bytes);
    }
}
