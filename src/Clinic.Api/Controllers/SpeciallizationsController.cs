using Clinic.Application.Features.Speciallization.Commands.Add;
using Clinic.Application.Features.Speciallization.Commands.Delete;
using Clinic.Application.Features.Speciallization.Commands.Update;
using Clinic.Application.Features.Speciallization.Queries.Get;
using Clinic.Application.Features.Speciallization.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class SpeciallizationsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await mediator.Send(new GetAllSpeciallizationsQuery(), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(uint id)
    {
        var result = await mediator.Send(new GetSpeciallizationQuery(id), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(uint id)
    {
        var result = await mediator.Send(new DeleteSpeciallizationCommand(id), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(AddSpeciallizationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(UpdateSpeciallizationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}