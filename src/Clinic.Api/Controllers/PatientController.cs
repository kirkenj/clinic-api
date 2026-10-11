using Clinic.Application.Features.Patients.Commands.Delete;
using Clinic.Application.Features.Patients.Commands.Update.Receptionist;
using Clinic.Application.Features.Patients.Queries.Get;
using Clinic.Application.Features.Patients.Queries.Get.Staff;
using Clinic.Application.Features.Patients.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PatientController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedPatientsAsync([FromQuery] PatientFilter? patientFilter,
        [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedPatientsRequest
        { PatientFilter = patientFilter, Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetPatientAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new StaffGetPatientQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePatientAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new DeletePatientCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPut]
    public async Task<IActionResult> UpdatePatientAsync([FromBody][Required] ReceptionistUpdatePatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}