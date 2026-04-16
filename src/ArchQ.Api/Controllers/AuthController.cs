using ArchQ.Application.Auth.Commands.Register;
using ArchQ.Application.Auth.Commands.VerifyEmail;
using ArchQ.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly IWebHostEnvironment _env;

    public AuthController(IMediator mediator, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _env = env;
    }

    // TODO: Add rate limiting to register endpoint. Deferred to Feature 20 (Security & Validation).
    [HttpPost("register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Register([FromBody] RegisterCommand command)
    {
        var response = await _mediator.Send(command);
        return StatusCode(StatusCodes.Status201Created, response);
    }

    [HttpPost("verify-email")]
    [ProducesResponseType(typeof(VerifyEmailResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailCommand command)
    {
        var response = await _mediator.Send(command);
        return Ok(response);
    }

    [HttpGet("test/verification-token")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetTestVerificationToken([FromQuery] string email)
    {
        if (!_env.IsDevelopment())
        {
            return NotFound();
        }

        var emailLower = email.Trim().ToLowerInvariant();
        if (SmtpEmailService.TestVerificationTokens.TryGetValue(emailLower, out var token))
        {
            return Ok(new { token });
        }

        return NotFound(new { message = "No verification token found for this email." });
    }
}
