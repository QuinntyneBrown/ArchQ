using ArchQ.Core.Interfaces;
using ArchQ.Infrastructure.Email;
using ArchQ.Infrastructure.Identity;
using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using ArchQ.Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ArchQ.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<CouchbaseConfiguration>(configuration.GetSection("Couchbase"));

        services.AddSingleton<CouchbaseContext>();

        services.AddScoped<ITenantRepository, TenantRepository>();
        services.AddScoped<IAuditRepository, AuditRepository>();
        services.AddScoped<ICouchbaseProvisioner, CouchbaseProvisioner>();
        services.AddSingleton<CouchbaseBootstrapper>();
        services.AddScoped<ITenantContext, TenantContext>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IGlobalUserRepository, GlobalUserRepository>();
        services.AddScoped<IVerificationTokenRepository, VerificationTokenRepository>();
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();
        services.AddScoped<IEmailService, SmtpEmailService>();

        return services;
    }
}
