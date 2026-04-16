using ArchQ.Application.Tenants.DTOs;
using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommandHandler : IRequestHandler<CreateTenantCommand, TenantResponse>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly ICouchbaseProvisioner _couchbaseProvisioner;
    private readonly IAuditRepository _auditRepository;

    public CreateTenantCommandHandler(
        ITenantRepository tenantRepository,
        ICouchbaseProvisioner couchbaseProvisioner,
        IAuditRepository auditRepository)
    {
        _tenantRepository = tenantRepository;
        _couchbaseProvisioner = couchbaseProvisioner;
        _auditRepository = auditRepository;
    }

    public async Task<TenantResponse> Handle(CreateTenantCommand request, CancellationToken cancellationToken)
    {
        if (await _tenantRepository.SlugExistsAsync(request.Slug))
        {
            throw new ConflictException("SLUG_CONFLICT", $"The organization slug '{request.Slug}' is already in use.");
        }

        await _couchbaseProvisioner.ProvisionScopeAsync(request.Slug);
        await _couchbaseProvisioner.CreateCollectionsAsync(request.Slug);
        // Create indexes in background to avoid blocking the HTTP response.
        // The Couchbase .NET SDK can hang when awaiting QueryAsync for DDL
        // statements on newly created collections.
        _ = Task.Run(async () =>
        {
            try
            {
                await _couchbaseProvisioner.CreateIndexesAsync(request.Slug);
            }
            catch
            {
                // Index creation is best-effort during provisioning.
                // Indexes will be retried on next query if missing.
            }
        });

        var tenant = new Tenant
        {
            DisplayName = request.DisplayName,
            Slug = request.Slug,
            Status = "active",
            Plan = "standard",
            CreatedBy = request.CreatedBy,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Settings = new TenantSettings()
        };

        var created = await _tenantRepository.CreateAsync(tenant);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = created.Id,
            Action = "TenantCreated",
            EntityType = "Tenant",
            EntityId = created.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"Tenant '{created.DisplayName}' created with slug '{created.Slug}'."
        });

        return TenantResponse.FromEntity(created);
    }
}
