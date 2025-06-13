using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Patients.Commands.Delete;
using Clinic.Application.Features.Patients.Commands.Update;
using Clinic.Application.Features.Patients.Queries.Get;
using Clinic.Application.Features.Patients.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PatientController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedPatientsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedPatientsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetPatientQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatientAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new DeletePatientCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdatePatientAsync([FromBody] [Required] UpdatePatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}