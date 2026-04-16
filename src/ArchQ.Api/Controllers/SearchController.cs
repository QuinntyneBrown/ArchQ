using ArchQ.Application.Adrs.Queries.SearchAdrs;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/search")]
public class SearchController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public SearchController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/search?q=...&amp;status=...&amp;pageSize=20&amp;offset=0
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(SearchResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> Search(
        string tenantSlug,
        [FromQuery] string q,
        [FromQuery] string? status,
        [FromQuery] int pageSize = 20,
        [FromQuery] int offset = 0)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        if (string.IsNullOrWhiteSpace(q) || q.Trim().Length < 2)
        {
            return BadRequest(new { code = "QUERY_TOO_SHORT", message = "Search query must be at least 2 characters." });
        }

        if (pageSize > 50)
        {
            pageSize = 50;
        }

        var response = await _mediator.Send(new SearchAdrsQuery
        {
            TenantSlug = tenantSlug,
            Query = q.Trim(),
            Status = status,
            PageSize = pageSize,
            Offset = offset
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
