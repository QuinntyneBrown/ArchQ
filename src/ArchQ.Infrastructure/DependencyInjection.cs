using ArchQ.Core.Interfaces;
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
        services.AddScoped<ITenantContext, TenantContext>();

        return services;
    }
}
