using ArchQ.Application.Tags.Commands.AssignTags;
using ArchQ.Application.Tags.Queries.ListTags;
using ArchQ.Application.Tags.Queries.SuggestTags;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}")]
public class TagsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public TagsController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/tags
    /// </summary>
    [HttpGet("tags")]
    [ProducesResponseType(typeof(ListTagsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListTags(string tenantSlug)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new ListTagsQuery { TenantSlug = tenantSlug });
        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/tags/autocomplete?q=
    /// </summary>
    [HttpGet("tags/autocomplete")]
    [ProducesResponseType(typeof(SuggestTagsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AutocompleteTags(string tenantSlug, [FromQuery] string q = "")
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new SuggestTagsQuery
        {
            TenantSlug = tenantSlug,
            Query = q
        });

        return Ok(response);
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/tags
    /// </summary>
    [HttpPost("adrs/{adrId}/tags")]
    [ProducesResponseType(typeof(AssignTagsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AssignTags(string tenantSlug, string adrId, [FromBody] AssignTagsRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new AssignTagsCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            Tags = request.Tags,
            ActorId = claims.UserId
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

public class AssignTagsRequest
{
    public List<string> Tags { get; set; } = new();
}
