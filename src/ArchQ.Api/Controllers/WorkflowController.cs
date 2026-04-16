using ArchQ.Application.Adrs.Commands.TransitionAdr;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/transitions")]
public class WorkflowController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public WorkflowController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/transitions
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(TransitionAdrResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> TransitionAdr(
        string tenantSlug,
        string adrId,
        [FromBody] TransitionAdrRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new TransitionAdrCommand
        {
            AdrId = adrId,
            TargetStatus = request.TargetStatus,
            ApproverIds = request.ApproverIds,
            SupersededBy = request.SupersededBy,
            Reason = request.Reason,
            TenantSlug = tenantSlug,
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

public class TransitionAdrRequest
{
    public string TargetStatus { get; set; } = string.Empty;
    public List<string>? ApproverIds { get; set; }
    public string? SupersededBy { get; set; }
    public string? Reason { get; set; }
}
