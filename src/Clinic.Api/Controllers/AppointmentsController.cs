using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Appointments.Commands.Add;
using Clinic.Application.Features.Appointments.Commands.Delete;
using Clinic.Application.Features.Appointments.Commands.SetApprovedByClinic;
using Clinic.Application.Features.Appointments.Commands.SetApprovedByPatient;
using Clinic.Application.Features.Appointments.Commands.Update;
using Clinic.Application.Features.Appointments.Queries.Get;
using Clinic.Application.Features.Appointments.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AppointmentsController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedAppointmentsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedAppointmentsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new GetAppointmentQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointmentAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new DeleteAppointmentCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointmentAsync([FromBody][Required] AddAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAppointmentAsync([FromBody][Required] UpdateAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("ApproveByClinic")]
    public async Task<IActionResult> ApproveRequestByClinic
        ([FromBody] [Required] SetApprovedAppointmentByClinicCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("ApproveByPatient")]
    public async Task<IActionResult> ApproveRequestByPatient
        ([FromBody] [Required] SetApprovedAppointmentByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}
