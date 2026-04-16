using ArchQ.Application.Audit.Queries.GetAuditEntry;
using ArchQ.Application.Audit.Queries.ListAuditEntries;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}")]
public class AuditController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly IAuditRepository _auditRepository;

    private const string AccessCookieName = "archq_access";

    public AuditController(IMediator mediator, ITokenService tokenService, IAuditRepository auditRepository)
    {
        _mediator = mediator;
        _tokenService = tokenService;
        _auditRepository = auditRepository;
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/audit
    /// </summary>
    [HttpGet("audit")]
    [ProducesResponseType(typeof(ListAuditEntriesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAuditEntries(
        string tenantSlug,
        [FromQuery] string? action,
        [FromQuery] string? resourceId,
        [FromQuery] string? actorId,
        [FromQuery] DateTime? dateFrom,
        [FromQuery] DateTime? dateTo,
        [FromQuery] int? pageSize,
        [FromQuery] int? offset)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new ListAuditEntriesQuery
        {
            TenantSlug = tenantSlug,
            Action = action,
            ResourceId = resourceId,
            ActorId = actorId,
            DateFrom = dateFrom,
            DateTo = dateTo,
            PageSize = pageSize ?? 25,
            Offset = offset ?? 0
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/audit/{id}
    /// </summary>
    [HttpGet("audit/{id}")]
    [ProducesResponseType(typeof(GetAuditEntryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetAuditEntry(string tenantSlug, string id)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetAuditEntryQuery
        {
            TenantSlug = tenantSlug,
            Id = id
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/audit
    /// </summary>
    [HttpGet("adrs/{adrId}/audit")]
    [ProducesResponseType(typeof(List<AuditEntryItem>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> ListAdrAudit(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var entries = await _auditRepository.ListByResourceAsync(adrId, tenantSlug);

        var items = entries.Select(e => new AuditEntryItem
        {
            Id = e.Id,
            Action = e.Action,
            EntityType = e.EntityType,
            EntityId = e.EntityId,
            UserId = e.UserId,
            Timestamp = e.Timestamp,
            Details = e.Details
        }).ToList();

        return Ok(new { items });
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
