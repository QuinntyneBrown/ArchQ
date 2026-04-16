using ArchQ.Application.Tenants.DTOs;
using MediatR;

namespace ArchQ.Application.Tenants.Commands.UpdateTenant;

public class UpdateTenantCommand : IRequest<TenantResponse>
{
    public string Id { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
}
