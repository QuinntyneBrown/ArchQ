using ArchQ.Application.Adrs.Commands.CreateAdr;
using ArchQ.Application.Adrs.Commands.UpdateAdr;
using ArchQ.Application.Adrs.DTOs;
using ArchQ.Application.Adrs.Queries.GetAdrById;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs")]
public class AdrsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public AdrsController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AdrResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> CreateAdr(string tenantSlug, [FromBody] CreateAdrRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new CreateAdrCommand
        {
            TenantSlug = tenantSlug,
            Title = request.Title,
            Body = request.Body,
            Tags = request.Tags,
            AuthorId = claims.UserId
        });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}
    /// </summary>
    [HttpGet("{adrId}")]
    [ProducesResponseType(typeof(AdrDetailResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAdrById(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetAdrByIdQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId
        });

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/tenants/{tenantSlug}/adrs/{adrId}
    /// </summary>
    [HttpPut("{adrId}")]
    [ProducesResponseType(typeof(UpdateAdrResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateAdr(string tenantSlug, string adrId, [FromBody] UpdateAdrRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new UpdateAdrCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            Title = request.Title,
            Body = request.Body,
            Tags = request.Tags,
            ActorId = claims.UserId,
            ActorRoles = claims.Roles
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

public class CreateAdrRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}

public class UpdateAdrRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
}
