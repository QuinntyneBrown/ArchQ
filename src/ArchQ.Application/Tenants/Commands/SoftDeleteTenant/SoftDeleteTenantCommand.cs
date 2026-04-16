using MediatR;

namespace ArchQ.Application.Tenants.Commands.SoftDeleteTenant;

public class SoftDeleteTenantCommand : IRequest<Unit>
{
    public string Id { get; set; } = string.Empty;
}
