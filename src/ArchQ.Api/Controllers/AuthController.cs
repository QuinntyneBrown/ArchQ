using ArchQ.Application.Auth.Commands.Login;
using ArchQ.Application.Auth.Commands.Logout;
using ArchQ.Application.Auth.Commands.RefreshToken;
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

    private const string AccessCookieName = "archq_access";
    private const string RefreshCookieName = "archq_refresh";

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

    [HttpPost("login")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        command.IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        command.UserAgent = Request.Headers.UserAgent.ToString();

        var response = await _mediator.Send(command);

        SetAuthCookies(response.AccessToken, response.RefreshToken);

        return Ok(new
        {
            user = response.User,
            tenant = response.Tenant,
            memberships = response.Memberships
        });
    }

    [HttpPost("refresh")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Refresh()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized(new { code = "MISSING_REFRESH_TOKEN", message = "No refresh token provided." });
        }

        var response = await _mediator.Send(new RefreshTokenCommand { RefreshToken = refreshToken });

        SetAuthCookies(response.AccessToken, response.RefreshToken);

        return Ok(new
        {
            user = response.User,
            tenant = response.Tenant,
            memberships = response.Memberships
        });
    }

    [HttpPost("logout")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Logout()
    {
        var refreshToken = Request.Cookies[RefreshCookieName];
        if (!string.IsNullOrEmpty(refreshToken))
        {
            await _mediator.Send(new LogoutCommand { RefreshToken = refreshToken });
        }

        ClearAuthCookies();

        return Ok(new { message = "Signed out successfully." });
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

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var isSecure = !_env.IsDevelopment();

        // Cookie Path is /api (not the narrower /api/auth/refresh) because the refresh
        // cookie must be sent to /api/orgs/switch so the handler can revoke the
        // previous refresh-token family during an org switch.
        Response.Cookies.Append(AccessCookieName, accessToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = "/api",
            MaxAge = TimeSpan.FromMinutes(15)
        });

        Response.Cookies.Append(RefreshCookieName, refreshToken, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = "/api",
            MaxAge = TimeSpan.FromDays(7)
        });
    }

    private void ClearAuthCookies()
    {
        var isSecure = !_env.IsDevelopment();

        Response.Cookies.Delete(AccessCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = "/api"
        });

        Response.Cookies.Delete(RefreshCookieName, new CookieOptions
        {
            HttpOnly = true,
            Secure = isSecure,
            SameSite = SameSiteMode.Strict,
            Path = "/api"
        });
    }
}
