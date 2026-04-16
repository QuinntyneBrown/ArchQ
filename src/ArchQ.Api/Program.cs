using ArchQ.Api.Middleware;
using ArchQ.Application;
using ArchQ.Infrastructure;
using ArchQ.Infrastructure.Persistence;

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
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// Bootstrap _system scope and its collections on startup
var bootstrapper = app.Services.GetRequiredService<CouchbaseBootstrapper>();
await bootstrapper.EnsureSystemScopeAsync();

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseCors();
app.UseHttpsRedirection();
app.MapControllers();

app.Run();
