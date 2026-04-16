using ArchQ.Api.Middleware;
using ArchQ.Application;
using ArchQ.Infrastructure;
using ArchQ.Infrastructure.Persistence;
using Microsoft.AspNetCore.Server.Kestrel.Core;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Kestrel request body size limit to 1MB default
builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = 1_048_576; // 1 MB
});

var app = builder.Build();

// Bootstrap system scope and its collections on startup
var bootstrapper = app.Services.GetRequiredService<CouchbaseBootstrapper>();
await bootstrapper.EnsureSystemScopeAsync();

app.UseMiddleware<SecurityHeadersMiddleware>();
app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
