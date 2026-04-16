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
            throw new ConflictException("SLUG_CONFLICT", $"A tenant with slug '{request.Slug}' already exists.");
        }

        await _couchbaseProvisioner.ProvisionScopeAsync(request.Slug);
        await _couchbaseProvisioner.CreateCollectionsAsync(request.Slug);
        await _couchbaseProvisioner.CreateIndexesAsync(request.Slug);

        var tenant = new Tenant
        {
            DisplayName = request.DisplayName,
            Slug = request.Slug,
            Status = "active",
            Plan = "standard",
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
