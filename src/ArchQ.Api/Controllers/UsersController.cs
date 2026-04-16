using ArchQ.Application.Users.Queries.ListUsers;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/users")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public UsersController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/users?role=reviewer
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ListUsersResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListUsers(string tenantSlug, [FromQuery] string? role)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new ListUsersQuery
        {
            TenantSlug = tenantSlug,
            Role = role
        });

        return Ok(response);
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
}
