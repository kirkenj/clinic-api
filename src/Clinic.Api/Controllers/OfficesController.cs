using Clinic.Application.Features.Office.Commands.AddOfficeCommand;
using Clinic.Application.Features.Office.Commands.UpdateOfficeCommand;
using Clinic.Application.Features.Office.Queries.GetAllOfficesQuery;
using Clinic.Application.Features.Office.Queries.GetOfficeQuery;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class OfficesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetAllOfficesQuery(), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest();
    }

    [HttpGet("{id}")]
    public async Task<ActionResult> GetAsync(uint id, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(new GetOfficeQuery(id), cancellationToken);
        if (result.IsSuccess)
        {
            return Ok(result.Value);
        }

        return BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult> PostAsync(AddOfficeCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<ActionResult> PutAsync(UpdateOfficeCommand request, CancellationToken cancellationToken)
    {
        var result = await mediator.Send(request, cancellationToken);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}