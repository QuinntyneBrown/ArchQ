using ArchQ.Application.Tenants.DTOs;
using MediatR;

namespace ArchQ.Application.Tenants.Commands.CreateTenant;

public class CreateTenantCommand : IRequest<TenantResponse>
{
    public string DisplayName { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string CreatedBy { get; set; } = string.Empty;
}
