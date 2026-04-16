using ArchQ.Application.Artifacts.Commands.UploadAttachment;
using ArchQ.Application.Artifacts.Queries.GetAttachments;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/attachments")]
public class AttachmentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public AttachmentsController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/attachments
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(AttachmentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [RequestSizeLimit(10 * 1024 * 1024)] // 10 MB
    public async Task<IActionResult> UploadAttachment(string tenantSlug, string adrId, IFormFile file, [FromForm] string? displayName, [FromForm] string? description)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        if (file is null || file.Length == 0)
        {
            return BadRequest(new { code = "BAD_REQUEST", message = "File is required." });
        }

        using var stream = file.OpenReadStream();

        var response = await _mediator.Send(new UploadAttachmentCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            FileName = file.FileName,
            DisplayName = displayName ?? Path.GetFileNameWithoutExtension(file.FileName),
            Description = description ?? string.Empty,
            ContentType = file.ContentType,
            FileSize = file.Length,
            FileStream = stream,
            AuthorId = claims.UserId
        });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/attachments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetAttachmentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetAttachments(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetAttachmentsQuery
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
