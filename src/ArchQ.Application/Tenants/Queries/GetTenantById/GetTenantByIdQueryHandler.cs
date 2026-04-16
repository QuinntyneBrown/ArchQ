using ArchQ.Application.Tenants.DTOs;
using ArchQ.Core.Exceptions;
using ArchQ.Core.Interfaces;
using MediatR;

namespace ArchQ.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQueryHandler : IRequestHandler<GetTenantByIdQuery, TenantResponse>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantByIdQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<TenantResponse> Handle(GetTenantByIdQuery request, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(request.Id)
            ?? throw new NotFoundException("TENANT_NOT_FOUND", $"Tenant with id '{request.Id}' was not found.");

        return TenantResponse.FromEntity(tenant);
    }
}
