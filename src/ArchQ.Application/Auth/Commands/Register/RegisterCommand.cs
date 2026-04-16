using MediatR;

namespace ArchQ.Application.Auth.Commands.Register;

public class RegisterCommand : IRequest<RegisterResponse>
{
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string OrganizationName { get; set; } = string.Empty;
    public string? InviteToken { get; set; }
}
