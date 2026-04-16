using ArchQ.Application.Adrs.Commands.RecordDecision;
using ArchQ.Application.Config.Commands.UpdateApprovalThreshold;
using ArchQ.Application.Config.Queries.GetApprovalThreshold;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}")]
public class ApprovalsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public ApprovalsController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/decisions
    /// </summary>
    [HttpPost("adrs/{adrId}/decisions")]
    [ProducesResponseType(typeof(RecordDecisionResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
    public async Task<IActionResult> RecordDecision(
        string tenantSlug,
        string adrId,
        [FromBody] RecordDecisionRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new RecordDecisionCommand
        {
            AdrId = adrId,
            TenantSlug = tenantSlug,
            UserId = claims.UserId,
            Decision = request.Decision,
            Comment = request.Comment
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/config/approval-threshold
    /// </summary>
    [HttpGet("config/approval-threshold")]
    [ProducesResponseType(typeof(ApprovalThresholdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetApprovalThreshold(string tenantSlug)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetApprovalThresholdQuery
        {
            TenantSlug = tenantSlug
        });

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/tenants/{tenantSlug}/config/approval-threshold
    /// </summary>
    [HttpPut("config/approval-threshold")]
    [ProducesResponseType(typeof(UpdateApprovalThresholdResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> UpdateApprovalThreshold(
        string tenantSlug,
        [FromBody] UpdateApprovalThresholdRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new UpdateApprovalThresholdCommand
        {
            TenantSlug = tenantSlug,
            ApprovalThreshold = request.ApprovalThreshold,
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

public class RecordDecisionRequest
{
    public string Decision { get; set; } = string.Empty;
    public string? Comment { get; set; }
}

public class UpdateApprovalThresholdRequest
{
    public int ApprovalThreshold { get; set; }
}
