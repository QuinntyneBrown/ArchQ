using ArchQ.Application.Artifacts.Commands.AddMeetingNote;
using ArchQ.Application.Artifacts.Queries.GetMeetingNotes;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/meeting-notes")]
public class MeetingNotesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public MeetingNotesController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/meeting-notes
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(MeetingNoteResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddMeetingNote(string tenantSlug, string adrId, [FromBody] AddMeetingNoteRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new AddMeetingNoteCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            Title = request.Title,
            MeetingDate = request.MeetingDate,
            Attendees = request.Attendees,
            Body = request.Body,
            AuthorId = claims.UserId
        });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/meeting-notes
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetMeetingNotesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetMeetingNotes(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetMeetingNotesQuery
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

public class AddMeetingNoteRequest
{
    public string Title { get; set; } = string.Empty;
    public DateTime MeetingDate { get; set; }
    public List<string> Attendees { get; set; } = new();
    public string Body { get; set; } = string.Empty;
}
