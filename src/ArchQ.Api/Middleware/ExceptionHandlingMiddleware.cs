using System.Text.Json;
using ArchQ.Core.Exceptions;
using FluentValidation;

namespace ArchQ.Api.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, error, message) = exception switch
        {
            NotFoundException ex => (StatusCodes.Status404NotFound, ex.Code, ex.Message),
            ForbiddenException ex => (StatusCodes.Status403Forbidden, ex.Code, ex.Message),
            ConflictException ex => (StatusCodes.Status409Conflict, ex.Code, ex.Message),
            ValidationException ex => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR", ex.Errors.FirstOrDefault()?.ErrorMessage ?? ex.Message),
            DomainException ex => (StatusCodes.Status400BadRequest, ex.Code, ex.Message),
            _ => (StatusCodes.Status500InternalServerError, "INTERNAL_ERROR", "An unexpected error occurred.")
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(exception, "Unhandled exception");
        }
        else
        {
            _logger.LogWarning(exception, "Handled exception: {Error}", error);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        object body;

        if (exception is ValidationException validationException)
        {
            var details = validationException.Errors
                .Select(e => new { field = e.PropertyName, message = e.ErrorMessage })
                .ToArray();

            body = new { error, message, details };
        }
        else
        {
            body = new { error, message };
        }

        var response = JsonSerializer.Serialize(body, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });

        await context.Response.WriteAsync(response);
    }
}
