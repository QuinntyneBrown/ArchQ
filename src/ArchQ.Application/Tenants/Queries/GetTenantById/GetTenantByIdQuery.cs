using ArchQ.Application.Tenants.DTOs;
using MediatR;

namespace ArchQ.Application.Tenants.Queries.GetTenantById;

public class GetTenantByIdQuery : IRequest<TenantResponse>
{
    public string Id { get; set; } = string.Empty;
}
