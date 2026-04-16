using System.CommandLine;
using ArchQ.Cli.Configuration;
using ArchQ.Cli.Handlers;
using ArchQ.Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ArchQ.Cli.Commands;

public static class SeedCommand
{
    public static Command Create(IServiceProvider services)
    {
        var command = new Command("seed", "Seed the database with initial tenants, users, and configuration");

        var connectionStringOption = new Option<string?>("--connection-string")
        {
            Description = "Couchbase connection string (default: couchbase://localhost)"
        };

        var usernameOption = new Option<string?>("--username")
        {
            Description = "Cluster administrator username (default: Administrator)"
        };

        var passwordOption = new Option<string?>("--password")
        {
            Description = "Cluster administrator password (default: password123)"
        };

        var bucketOption = new Option<string?>("--bucket")
        {
            Description = "Bucket name (default: archq)"
        };

        var adminEmailOption = new Option<string?>("--admin-email")
        {
            Description = "Admin user email address (default: admin@archq.local)"
        };

        var adminPasswordOption = new Option<string?>("--admin-password")
        {
            Description = "Admin user password (default: Admin@123)"
        };

        var adminNameOption = new Option<string?>("--admin-name")
        {
            Description = "Admin user full name (default: System Administrator)"
        };

        var tenantNameOption = new Option<string?>("--tenant-name")
        {
            Description = "Default tenant display name (default: Default Organization)"
        };

        var tenantSlugOption = new Option<string?>("--tenant-slug")
        {
            Description = "Default tenant slug (default: default)"
        };

        command.Add(connectionStringOption);
        command.Add(usernameOption);
        command.Add(passwordOption);
        command.Add(bucketOption);
        command.Add(adminEmailOption);
        command.Add(adminPasswordOption);
        command.Add(adminNameOption);
        command.Add(tenantNameOption);
        command.Add(tenantSlugOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var connectionString = parseResult.GetValue(connectionStringOption);
            var username = parseResult.GetValue(usernameOption);
            var password = parseResult.GetValue(passwordOption);
            var bucket = parseResult.GetValue(bucketOption);
            var adminEmail = parseResult.GetValue(adminEmailOption);
            var adminPassword = parseResult.GetValue(adminPasswordOption);
            var adminName = parseResult.GetValue(adminNameOption);
            var tenantName = parseResult.GetValue(tenantNameOption);
            var tenantSlug = parseResult.GetValue(tenantSlugOption);

            var cbConfig = services.GetRequiredService<IOptions<CouchbaseConfiguration>>().Value;
            if (connectionString is not null) cbConfig.ConnectionString = connectionString;
            if (username is not null) cbConfig.Username = username;
            if (password is not null) cbConfig.Password = password;
            if (bucket is not null) cbConfig.BucketName = bucket;

            var seedConfig = services.GetRequiredService<IOptions<SeedConfiguration>>().Value;
            if (adminEmail is not null) seedConfig.AdminEmail = adminEmail;
            if (adminPassword is not null) seedConfig.AdminPassword = adminPassword;
            if (adminName is not null) seedConfig.AdminName = adminName;
            if (tenantName is not null) seedConfig.TenantName = tenantName;
            if (tenantSlug is not null) seedConfig.TenantSlug = tenantSlug;

            var handler = services.GetRequiredService<SeedHandler>();
            return await handler.ExecuteAsync(cancellationToken);
        });

        return command;
    }
}
