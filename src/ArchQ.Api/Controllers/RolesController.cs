using ArchQ.Application.Roles.Commands.AddRole;
using ArchQ.Application.Roles.Commands.RemoveRole;
using ArchQ.Application.Roles.Commands.SetRoles;
using ArchQ.Application.Roles.DTOs;
using ArchQ.Application.Roles.Queries.GetUserRoles;
using ArchQ.Core.Authorization;
using ArchQ.Core.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/tenants/{tenantId}/users/{userId}/roles")]
public class RolesController : ControllerBase
{
    private readonly IMediator _mediator;
    private readonly ITokenService _tokenService;

    private const string AccessCookieName = "archq_access";

    public RolesController(IMediator mediator, ITokenService tokenService)
    {
        _mediator = mediator;
        _tokenService = tokenService;
    }

    /// <summary>
    /// GET /api/tenants/{tenantId}/users/{userId}/roles
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(UserRolesResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    public async Task<IActionResult> GetUserRoles(string tenantId, string userId)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        var response = await _mediator.Send(new GetUserRolesQuery
        {
            TenantSlug = tenantId,
            UserId = userId
        });

        return Ok(response);
    }

    /// <summary>
    /// PUT /api/tenants/{tenantId}/users/{userId}/roles — full replace
    /// </summary>
    [HttpPut]
    [ProducesResponseType(typeof(RoleChangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> SetRoles(string tenantId, string userId, [FromBody] SetRolesRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        if (!RolePolicy.CanPerform(claims.Roles, "role.assign"))
        {
            return Forbid();
        }

        // Self-promotion prevention: admin cannot assign/modify their own roles
        if (string.Equals(claims.UserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { code = "SELF_PROMOTION", message = "Admins cannot modify their own roles. Another admin must do this." });
        }

        var response = await _mediator.Send(new SetRolesCommand
        {
            TenantSlug = tenantId,
            UserId = userId,
            Roles = request.Roles,
            ActorId = claims.UserId
        });

        return Ok(response);
    }

    /// <summary>
    /// POST /api/tenants/{tenantId}/users/{userId}/roles — add a single role
    /// </summary>
    [HttpPost]
    [ProducesResponseType(typeof(RoleChangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> AddRole(string tenantId, string userId, [FromBody] AddRoleRequest request)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        if (!RolePolicy.CanPerform(claims.Roles, "role.assign"))
        {
            return Forbid();
        }

        // Self-promotion prevention: admin cannot assign roles to themselves
        if (string.Equals(claims.UserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { code = "SELF_PROMOTION", message = "Admins cannot modify their own roles. Another admin must do this." });
        }

        var response = await _mediator.Send(new AddRoleCommand
        {
            TenantSlug = tenantId,
            UserId = userId,
            Role = request.Role,
            ActorId = claims.UserId
        });

        return Ok(response);
    }

    /// <summary>
    /// DELETE /api/tenants/{tenantId}/users/{userId}/roles/{role}
    /// </summary>
    [HttpDelete("{role}")]
    [ProducesResponseType(typeof(RoleChangeResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> RemoveRole(string tenantId, string userId, string role)
    {
        var claims = GetAccessTokenClaims();
        if (claims is null)
        {
            return Unauthorized(new { code = "INVALID_TOKEN", message = "Access token is missing or invalid." });
        }

        if (!RolePolicy.CanPerform(claims.Roles, "role.assign"))
        {
            return Forbid();
        }

        // Self-promotion prevention: admin cannot remove their own roles
        if (string.Equals(claims.UserId, userId, StringComparison.OrdinalIgnoreCase))
        {
            return StatusCode(StatusCodes.Status403Forbidden,
                new { code = "SELF_PROMOTION", message = "Admins cannot modify their own roles. Another admin must do this." });
        }

        var response = await _mediator.Send(new RemoveRoleCommand
        {
            TenantSlug = tenantId,
            UserId = userId,
            Role = role,
            ActorId = claims.UserId
        });

        return Ok(response);
    }

    /// <summary>
    /// GET /api/tenants/{tenantId}/roles — static role definitions
    /// </summary>
    [HttpGet("/api/tenants/{tenantId}/roles")]
    [ProducesResponseType(typeof(List<RoleDefinition>), StatusCodes.Status200OK)]
    public IActionResult GetRoleDefinitions(string tenantId)
    {
        var definitions = RolePolicy.ValidRoles.Select(role => new RoleDefinition(
            Role: role,
            DisplayName: char.ToUpperInvariant(role[0]) + role[1..],
            Description: role switch
            {
                "admin" => "Full access to all tenant settings, user management, and ADR operations.",
                "author" => "Can create and edit ADRs and participate in discussions.",
                "reviewer" => "Can approve ADRs and participate in discussions.",
                "viewer" => "Read-only access to ADRs.",
                _ => string.Empty
            },
            Permissions: RolePolicy.Permissions[role].ToList()
        )).ToList();

        return Ok(definitions);
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

public class SetRolesRequest
{
    public List<string> Roles { get; set; } = new();
}

public class AddRoleRequest
{
    public string Role { get; set; } = string.Empty;
}
