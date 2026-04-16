using ArchQ.Application.Tenants.Commands.CreateTenant;
using ArchQ.Application.Tenants.Commands.UpdateTenant;
using ArchQ.Application.Tenants.DTOs;
using ArchQ.Application.Tenants.Queries.GetTenantById;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace ArchQ.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TenantsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status201Created)]
    public async Task<IActionResult> Create([FromBody] CreateTenantCommand command)
    {
        var tenant = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = tenant.Id }, tenant);
    }

    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetById(string id)
    {
        var tenant = await _mediator.Send(new GetTenantByIdQuery { Id = id });
        return Ok(tenant);
    }

    [HttpPatch("{id}")]
    [ProducesResponseType(typeof(TenantResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> Update(string id, [FromBody] UpdateTenantCommand command)
    {
        command.Id = id;
        var tenant = await _mediator.Send(command);
        return Ok(tenant);
    }
}
