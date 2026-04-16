using MediatR;

namespace ArchQ.Application.Auth.Commands.Logout;

public class LogoutCommand : IRequest<LogoutResponse>
{
    public string RefreshToken { get; set; } = string.Empty;
}
