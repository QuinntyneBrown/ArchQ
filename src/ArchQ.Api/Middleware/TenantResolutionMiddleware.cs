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
        // TODO: Implement JWT-based tenant resolution when auth feature is added.
        await _next(context);
    }
}
