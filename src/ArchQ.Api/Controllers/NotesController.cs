using ArchQ.Application.Artifacts.Commands.AddGeneralNote;
using ArchQ.Application.Artifacts.Queries.GetGeneralNotes;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/notes")]
public class NotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public NotesController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/notes
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(GeneralNoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddNote(string tenantSlug, string adrId, [FromBody] AddNoteRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new AddGeneralNoteCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            Title = request.Title,
            Body = request.Body,
            AuthorId = claims.UserId
        });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/notes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetGeneralNotesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetNotes(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetGeneralNotesQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId
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

public class AddNoteRequest
{
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
}
