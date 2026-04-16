using MediatR;

namespace ArchQ.Application.Auth.Commands.VerifyEmail;

public class VerifyEmailCommand : IRequest<VerifyEmailResponse>
{
    public string Token { get; set; } = string.Empty;
}
