using ArchQ.Application.Orgs.Commands.SwitchOrg;
using ArchQ.Application.Orgs.Queries.ListMemberships;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrgsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IWebHostEnvironment _env;

    private const string AccessCookieName = "archq_access";
    private const string RefreshCookieName = "archq_refresh";

    public OrgsController(IMediator mediator, ITokenService tokenService, IWebHostEnvironment env)
    {
        _mediator = mediator;
        _tokenService = tokenService;
        _env = env;
    }

    [HttpGet]
    [ProducesResponseType(typeof(ListMembershipsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListMemberships()
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new ListMembershipsQuery
        {
            Email = claims.Email,
            CurrentTenantSlug = claims.TenantSlug
        });

        return Ok(response);
    }

    [HttpPost("switch")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SwitchOrg([FromBody] SwitchOrgRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var refreshToken = Request.Cookies[RefreshCookieName] ?? string.Empty;

        var response = await _mediator.Send(new SwitchOrgCommand
        {
            TenantSlug = request.TenantSlug,
            Email = claims.Email,
            CurrentRefreshToken = refreshToken,
            IpAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            UserAgent = Request.Headers.UserAgent.ToString()
        });

        SetAuthCookies(response.AccessToken, response.RefreshToken);

        return Ok(new
        {
            tenant = response.Tenant,
            user = response.User
        });
    }

    private AccessTokenPayload? GetAccessTokenClaims()
    {
        var accessToken = Request.Cookies[AccessCookieName];
        if (string.IsNullOrEmpty(accessToken))
        {
            return null;
        }

        return _tokenService.VerifyAccessToken(accessToken);
    }

    private void SetAuthCookies(string accessToken, string refreshToken)
    {
        var isSecure = !_env.IsDevelopment();

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
}

public class SwitchOrgRequest
{
    public string TenantSlug { get; set; } = string.Empty;
}
