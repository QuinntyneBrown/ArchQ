using ArchQ.Application.Adrs.DTOs;
using MediatR;

namespace ArchQ.Application.Adrs.Commands.CreateAdr;

public class CreateAdrCommand : IRequest<AdrResponse>
{
    public string TenantSlug { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public List<string> Tags { get; set; } = new();
    public string AuthorId { get; set; } = string.Empty;
}
