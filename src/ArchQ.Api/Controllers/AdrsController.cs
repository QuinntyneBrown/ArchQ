using ArchQ.Application.Adrs.Commands.CreateAdr;
using ArchQ.Application.Adrs.DTOs;
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
