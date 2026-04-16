using ArchQ.Core.Entities;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tenants.Commands.SoftDeleteTenant;

public class SoftDeleteTenantCommandHandler : IRequestHandler<SoftDeleteTenantCommand, Unit>
{
    private readonly ITenantRepository _tenantRepository;
    private readonly IAuditRepository _auditRepository;

    public SoftDeleteTenantCommandHandler(
        ITenantRepository tenantRepository,
        IAuditRepository auditRepository)
    {
        _tenantRepository = tenantRepository;
        _auditRepository = auditRepository;
    }

    public async Task<Unit> Handle(SoftDeleteTenantCommand request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("TENANT_NOT_FOUND", $"Tenant with id '{request.Id}' was not found.");

        await _tenantRepository.SoftDeleteAsync(request.Id);

        await _auditRepository.WriteEntryAsync(new AuditEntry
        {
            TenantId = tenant.Id,
            Action = "TenantDeleted",
            EntityType = "Tenant",
            EntityId = tenant.Id,
            Timestamp = DateTime.UtcNow,
            Details = $"Tenant '{tenant.DisplayName}' soft-deleted."
        });

        return Unit.Value;
    }
}
