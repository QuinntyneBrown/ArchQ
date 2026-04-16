using System.Security.Cryptography;
using System.Text;
using ArchQ.Cli.Configuration;
using ArchQ.Core.Entities;
using ArchQ.Core.Interfaces;
using Couchbase.Core.Exceptions.KeyValue;
using ArchQ.Infrastructure.Persistence;
using ArchQ.Infrastructure.Persistence.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace ArchQ.Cli.Handlers;

public class SeedHandler
{
    private readonly CouchbaseContext _couchbaseContext;
    private readonly ICouchbaseProvisioner _provisioner;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IOptions<CouchbaseConfiguration> _couchbaseConfig;
    private readonly IOptions<SeedConfiguration> _seedConfig;
    private readonly ILogger<SeedHandler> _logger;

    public SeedHandler(
        CouchbaseContext couchbaseContext,
        ICouchbaseProvisioner provisioner,
        IPasswordHasher passwordHasher,
        IOptions<CouchbaseConfiguration> couchbaseConfig,
        IOptions<SeedConfiguration> seedConfig,
        ILogger<SeedHandler> logger)
    {
        _couchbaseContext = couchbaseContext;
        _provisioner = provisioner;
        _passwordHasher = passwordHasher;
        _couchbaseConfig = couchbaseConfig;
        _seedConfig = seedConfig;
        _logger = logger;
    }

    public async Task<int> ExecuteAsync(CancellationToken cancellationToken)
    {
        var seed = _seedConfig.Value;

        _logger.LogInformation("Seeding database with tenant '{TenantName}' (slug: {TenantSlug})", seed.TenantName, seed.TenantSlug);

        // Step 1: Create tenant document
        var tenant = new Tenant
        {
            Id = Guid.NewGuid().ToString(),
            DisplayName = seed.TenantName,
            Slug = seed.TenantSlug,
            Status = "active",
            Plan = "standard",
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var tenantsCollection = await _couchbaseContext.GetCollectionAsync("_system", "tenants");
        var tenantKey = $"tenant::{tenant.Id}";

        try
        {
            await tenantsCollection.InsertAsync(tenantKey, tenant);
            _logger.LogInformation("Created tenant '{TenantName}' with id {TenantId}", tenant.DisplayName, tenant.Id);
        }
        catch (DocumentExistsException)
        {
            _logger.LogWarning("Tenant document {TenantKey} already exists; skipping", tenantKey);
        }

        // Step 2: Provision tenant scope
        _logger.LogInformation("Provisioning scope '{Slug}'...", seed.TenantSlug);
        await _provisioner.ProvisionScopeAsync(seed.TenantSlug);

        // Step 3: Create tenant collections
        _logger.LogInformation("Creating tenant collections...");
        await _provisioner.CreateCollectionsAsync(seed.TenantSlug);

        // Step 4: Wait for collections to propagate
        _logger.LogInformation("Waiting for collections to propagate...");
        await Task.Delay(5000, cancellationToken);

        // Step 5: Create indexes
        _logger.LogInformation("Creating indexes...");
        await _provisioner.CreateIndexesAsync(seed.TenantSlug);

        // Step 6: Create admin user
        var userId = Guid.NewGuid().ToString();
        var user = new User
        {
            Id = userId,
            Email = seed.AdminEmail,
            FullName = seed.AdminName,
            PasswordHash = _passwordHasher.Hash(seed.AdminPassword),
            Status = "active",
            EmailVerified = true,
            Roles = ["admin"],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var usersCollection = await _couchbaseContext.GetCollectionAsync(seed.TenantSlug, "users");
        var userKey = $"user::{userId}";

        try
        {
            await usersCollection.InsertAsync(userKey, user);
            _logger.LogInformation("Created admin user '{Email}' with id {UserId}", seed.AdminEmail, userId);
        }
        catch (DocumentExistsException)
        {
            _logger.LogWarning("User document {UserKey} already exists; skipping", userKey);
        }

        // Step 7: Create global user entry
        var emailHash = Convert.ToHexStringLower(
            SHA256.HashData(Encoding.UTF8.GetBytes(seed.AdminEmail.ToLowerInvariant())));
        var globalUserKey = $"guser::{emailHash}";

        var globalUser = new GlobalUser
        {
            Email = seed.AdminEmail,
            Memberships =
            [
                new GlobalUserMembership
                {
                    TenantId = tenant.Id,
                    TenantSlug = seed.TenantSlug,
                    UserId = userId,
                    Status = "active"
                }
            ],
            CreatedAt = DateTime.UtcNow
        };

        var globalUsersCollection = await _couchbaseContext.GetCollectionAsync("_system", "global_users");

        try
        {
            await globalUsersCollection.InsertAsync(globalUserKey, globalUser);
            _logger.LogInformation("Created global user entry for '{Email}'", seed.AdminEmail);
        }
        catch (DocumentExistsException)
        {
            _logger.LogWarning("Global user {GlobalUserKey} already exists; skipping", globalUserKey);
        }

        // Step 8: Seed default ADR template
        var template = new TemplateConfig
        {
            Id = Guid.NewGuid().ToString(),
            Key = "adr_template",
            Body = """
                   ## Status

                   Proposed

                   ## Context

                   [Describe the context and problem statement]

                   ## Decision

                   [Describe the decision that was made]

                   ## Consequences

                   [Describe the consequences of this decision]
                   """,
            RequiredSections = ["Context", "Decision", "Consequences"],
            ApprovalThreshold = 1,
            UpdatedAt = DateTime.UtcNow,
            UpdatedBy = "system"
        };

        var configCollection = await _couchbaseContext.GetCollectionAsync(seed.TenantSlug, "config");
        var configKey = $"config::adr_template";

        try
        {
            await configCollection.InsertAsync(configKey, template);
            _logger.LogInformation("Seeded default ADR template");
        }
        catch (DocumentExistsException)
        {
            _logger.LogWarning("ADR template config already exists; skipping");
        }

        _logger.LogInformation("Database seeding complete");
        _logger.LogInformation("  Tenant:     {TenantName} ({TenantSlug})", seed.TenantName, seed.TenantSlug);
        _logger.LogInformation("  Admin user: {AdminEmail}", seed.AdminEmail);
        _logger.LogInformation("  Admin pass: {AdminPassword}", seed.AdminPassword);

        return 0;
    }
}
