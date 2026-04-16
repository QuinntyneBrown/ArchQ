using System.Security.Claims;
using ArchQ.Infrastructure.Identity;

namespace ArchQ.Api.Middleware;

public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value ?? string.Empty;

        // Skip tenant resolution for tenant management endpoints
        if (path.StartsWith("/api/tenants", StringComparison.OrdinalIgnoreCase))
        {
            await _next(context);
            return;
        }

        var user = context.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            var tenantId = user.FindFirstValue("tenant_id");
            var tenantSlug = user.FindFirstValue("tenant_slug");

            if (string.IsNullOrEmpty(tenantId) || string.IsNullOrEmpty(tenantSlug))
            {
                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsJsonAsync(new { error = "MISSING_TENANT_CLAIMS", message = "JWT is missing required tenant claims." });
                return;
            }

            var tenantContext = context.RequestServices.GetRequiredService<TenantContext>();
            tenantContext.TenantId = tenantId;
            tenantContext.TenantSlug = tenantSlug;
        }

        await _next(context);
    }
}
