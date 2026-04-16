using ArchQ.Core.Interfaces;
using ArchQ.Application.Common;
using MediatR;

namespace ArchQ.Application.Auth.Commands.Logout;

public class LogoutCommandHandler : IRequestHandler<LogoutCommand, LogoutResponse>
{
    private readonly ITokenService _tokenService;
    private readonly IRefreshTokenRepository _refreshTokenRepository;

    public LogoutCommandHandler(
        ITokenService tokenService,
        IRefreshTokenRepository refreshTokenRepository)
    {
        _tokenService = tokenService;
        _refreshTokenRepository = refreshTokenRepository;
    }

    public async Task<LogoutResponse> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var payload = _tokenService.VerifyRefreshToken(request.RefreshToken);
        if (payload is not null)
        {
            var tokenHash = TokenHasher.ComputeSha256(payload.TokenId);
            var storedToken = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);

            if (storedToken is not null && !storedToken.Revoked)
            {
                storedToken.Revoked = true;
                storedToken.RevokedAt = DateTime.UtcNow;
                await _refreshTokenRepository.UpdateAsync(storedToken);
            }
        }

        return new LogoutResponse
        {
            Message = "Logged out successfully."
        };
    }
}
