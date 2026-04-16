using ArchQ.Application.Adrs.Commands.UpdateTemplate;
using ArchQ.Application.Adrs.Queries.GetTemplate;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/config/template")]
public class TemplateController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public TemplateController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/config/template
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(TemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetTemplate(string tenantSlug)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetTemplateQuery
        {
            TenantSlug = tenantSlug
        });

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/tenants/{tenantSlug}/config/template
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(TemplateResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateTemplate(string tenantSlug, [FromBody] UpdateTemplateRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new UpdateTemplateCommand
        {
            TenantSlug = tenantSlug,
            Body = request.Body,
            RequiredSections = request.RequiredSections,
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

public class UpdateTemplateRequest
{
    public string Body { get; set; } = string.Empty;
    public List<string> RequiredSections { get; set; } = new();
}
