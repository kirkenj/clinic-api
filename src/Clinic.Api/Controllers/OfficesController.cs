using Clinic.Application.Features.Office.Commands.Add;
using Clinic.Application.Features.Office.Commands.Delete;
using Clinic.Application.Features.Office.Commands.Update;
using Clinic.Application.Features.Office.Queries.GetAll;
using Clinic.Application.Features.Office.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class OfficesController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await mediator.Send(new GetAllOfficesQuery(), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(uint id)
    {
        var result = await mediator.Send(new GetOfficeQuery(id), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(uint id)
    {
        var result = await mediator.Send(new DeleteOfficeCommand(id), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> PostAsync(AddOfficeCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> PutAsync(UpdateOfficeCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}