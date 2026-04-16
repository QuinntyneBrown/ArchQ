using ArchQ.Application.Adrs.Queries.CompareVersions;
using ArchQ.Application.Adrs.Queries.GetVersion;
using ArchQ.Application.Adrs.Queries.ListVersions;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/versions")]
public class VersionsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public VersionsController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/versions
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(ListVersionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListVersions(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new ListVersionsQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/versions/{version}
    /// </summary>
    [HttpGet("{version:int}")]
    [ProducesResponseType(typeof(GetVersionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetVersion(string tenantSlug, string adrId, int version)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetVersionQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            Version = version
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/versions/compare?from=1&amp;to=2
    /// </summary>
    [HttpGet("compare")]
    [ProducesResponseType(typeof(CompareVersionsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> CompareVersions(string tenantSlug, string adrId, [FromQuery] int from, [FromQuery] int to)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new CompareVersionsQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            FromVersion = from,
            ToVersion = to
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
