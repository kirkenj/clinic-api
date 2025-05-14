using Clinic.Application.Features.Patients.Commands.Add;
using Clinic.Application.Features.Patients.Commands.Delete;
using Clinic.Application.Features.Patients.Commands.Update;
using Clinic.Application.Features.Patients.Queries.Get;
using Clinic.Application.Features.Patients.Queries.GetAll;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;


[Route("api/[controller]")]
[ApiController]
public sealed class PatientsController(IMediator mediator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllAsync()
    {
        var result = await mediator.Send(new GetAllPatientsQuery(), HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAsync(uint id)
    {
        var result = await mediator.Send(new GetPatientQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAsync(uint id)
    {
        var result = await mediator.Send(new DeletePatientCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddAsync(AddPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAsync(UpdatePatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}