using ArchQ.Application.Tenants.DTOs;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommandHandler : IRequestHandler<UpdateTenantCommand, TenantResponse>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuditRepository _auditRepository;

    public UpdateTenantCommandHandler(
        ITenantRepository tenantRepository,
        IAuditRepository auditRepository)
    {
        _tenantRepository = tenantRepository;
        _auditRepository = auditRepository;
    }

    public async Task<TenantResponse> Handle(UpdateTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("TENANT_NOT_FOUND", $"Tenant with id '{request.Id}' was not found.");

        tenant.DisplayName = request.DisplayName;
        tenant.UpdatedAt = DateTime.UtcNow;

        var updated = await _tenantRepository.UpdateAsync(tenant);

        await _auditRepository.WriteEntryAsync(new Core.Entities.AuditEntry
        {
            TenantId = updated.Id,
            Action = "TenantUpdated",
            EntityType = "Tenant",
            EntityId = updated.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"Tenant display name updated to '{updated.DisplayName}'."
        });

        return TenantResponse.FromEntity(updated);
    }
}
