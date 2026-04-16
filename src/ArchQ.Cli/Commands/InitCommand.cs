using System.CommandLine;
using ArchQ.Cli.Handlers;
using ArchQ.Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace ArchQ.Cli.Commands;

public static class InitCommand
{
    public static Command Create(IServiceProvider services)
    {
        var command = new Command("init", "Initialize a fresh Couchbase cluster for ArchQ");

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
            Description = "Bucket name to create (default: archq)"
        };

        command.Add(connectionStringOption);
        command.Add(usernameOption);
        command.Add(passwordOption);
        command.Add(bucketOption);

        command.SetAction(async (parseResult, cancellationToken) =>
        {
            var connectionString = parseResult.GetValue(connectionStringOption);
            var username = parseResult.GetValue(usernameOption);
            var password = parseResult.GetValue(passwordOption);
            var bucket = parseResult.GetValue(bucketOption);

            var config = services.GetRequiredService<IOptions<CouchbaseConfiguration>>().Value;
            if (connectionString is not null) config.ConnectionString = connectionString;
            if (username is not null) config.Username = username;
            if (password is not null) config.Password = password;
            if (bucket is not null) config.BucketName = bucket;

            var handler = services.GetRequiredService<InitHandler>();
            return await handler.ExecuteAsync(cancellationToken);
        });

        return command;
    }
}
