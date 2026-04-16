using System.CommandLine;
using ArchQ.Cli.Commands;
using ArchQ.Cli.Configuration;
using ArchQ.Cli.Handlers;
using ArchQ.Core.Interfaces;
using ArchQ.Infrastructure.Identity;
using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
    .AddEnvironmentVariables("ARCHQ_");

builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.Configure<CouchbaseConfiguration>(builder.Configuration.GetSection("Couchbase"));
builder.Services.Configure<SeedConfiguration>(builder.Configuration.GetSection("Seed"));

builder.Services.AddHttpClient();
builder.Services.AddSingleton<CouchbaseContext>();
builder.Services.AddSingleton<CouchbaseBootstrapper>();
builder.Services.AddSingleton<ICouchbaseProvisioner, CouchbaseProvisioner>();
builder.Services.AddSingleton<IPasswordHasher, PasswordHasher>();
builder.Services.AddSingleton<InitHandler>();
builder.Services.AddSingleton<SeedHandler>();

using var host = builder.Build();

var rootCommand = new RootCommand("ArchQ database management CLI");
rootCommand.Add(InitCommand.Create(host.Services));
rootCommand.Add(SeedCommand.Create(host.Services));

return await rootCommand.Parse(args).InvokeAsync();
