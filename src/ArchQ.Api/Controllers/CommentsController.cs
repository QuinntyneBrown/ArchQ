using ArchQ.Application.Artifacts.Commands.AddComment;
using ArchQ.Application.Artifacts.Commands.EditComment;
using ArchQ.Application.Artifacts.Queries.GetComments;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantSlug}/adrs/{adrId}/comments")]
public class CommentsController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;
    private readonly ICommentRepository _commentRepository;

    private const string AccessCookieName = "archq_access";

    public CommentsController(IMediator mediator, ITokenService tokenService, ICommentRepository commentRepository)
    {
        _mediator = mediator;
        _tokenService = tokenService;
        _commentRepository = commentRepository;
    }

    /// <summary>
    /// POST /api/tenants/{tenantSlug}/adrs/{adrId}/comments
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> AddComment(string tenantSlug, string adrId, [FromBody] AddCommentRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new AddCommentCommand
        {
            TenantSlug = tenantSlug,
            AdrId = adrId,
            ParentCommentId = request.ParentCommentId,
            Body = request.Body,
            AuthorId = claims.UserId
        });

        return StatusCode(StatusCodes.Status201Created, response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantSlug}/adrs/{adrId}/comments
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(GetCommentsResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetComments(string tenantSlug, string adrId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetCommentsQuery
        {
            TenantSlug = tenantSlug,
            AdrId = adrId
        });

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/tenants/{tenantSlug}/adrs/{adrId}/comments/{commentId}
    /// </summary>
    [HttpPut("{commentId}")]
    [ProducesResponseType(typeof(CommentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> EditComment(string tenantSlug, string adrId, string commentId, [FromBody] EditCommentRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new EditCommentCommand
        {
            TenantSlug = tenantSlug,
            CommentId = commentId,
            Body = request.Body,
            ActorId = claims.UserId
        });

        return Ok(response);
    }

    /// <summary>
    /// DELETE /api/tenants/{tenantSlug}/adrs/{adrId}/comments/{commentId}
    /// </summary>
    [HttpDelete("{commentId}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> DeleteComment(string tenantSlug, string adrId, string commentId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var comment = await _commentRepository.GetByIdAsync(commentId, tenantSlug);
        if (comment is null)
        {
            return NotFound(new { code = "NOT_FOUND", message = "Comment not found." });
        }

        if (comment.AuthorId != claims.UserId)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { code = "FORBIDDEN", message = "Only the comment author can delete a comment." });
        }

        // Soft delete — mark as deleted rather than removing the document
        comment.Deleted = true;
        comment.Body = "[deleted]";
        comment.UpdatedAt = DateTime.UtcNow;
        await _commentRepository.UpdateAsync(comment, tenantSlug);

        return NoContent();
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

public class AddCommentRequest
{
    public string? ParentCommentId { get; set; }
    public string Body { get; set; } = string.Empty;
}

public class EditCommentRequest
{
    public string Body { get; set; } = string.Empty;
}
