using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Appointments.Commands.Add;
using Clinic.Application.Features.Appointments.Commands.ApproveByClinic;
using Clinic.Application.Features.Appointments.Commands.ApproveByPatient;
using Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;
using Clinic.Application.Features.Appointments.Commands.CancelByClinic;
using Clinic.Application.Features.Appointments.Commands.CancelByPatient;
using Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;
using Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;
using Clinic.Application.Features.Appointments.Queries.Get;
using Clinic.Application.Features.Appointments.Queries.GetForDoctorOnDate;
using Clinic.Application.Features.Appointments.Queries.GetPaginated;
using Clinic.Application.Features.Appointments.Queries.GetPaginatedForDoctor;
using Clinic.Application.Features.Appointments.Queries.GetPaginatedForPatient;
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

    [HttpGet("patientPaged")]
    public async Task<IActionResult> GetPaginatedAppointmentsForPatentAsync([FromQuery][Required] GetPaginatedAppointmentsForPatientRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("doctorPaged")]
    public async Task<IActionResult> GetPaginatedAppointmentsForDoctorAsync([FromQuery][Required] GetPaginatedAppointmentsForDoctorRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("datedDoctorRequests")]
    public async Task<IActionResult> GetPaginatedAppointmentsForDoctorAsync([FromQuery][Required] GetRequestsForDoctorOnDateRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetAppointmentQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointmentAsync([FromBody] [Required] AddAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("SendOtpToConfirmAppointment")]
    public async Task<IActionResult> SendOtpToConfirmAppointment(SendOtpToCreateAppointmentRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("ApproveByPatient")]
    public async Task<IActionResult> ApproveRequestByPatient
        ([FromBody] [Required] ApproveAppointmentByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("ApproveByClinic")]
    public async Task<IActionResult> ApproveRequestByClinic
        ([FromBody] [Required] ApproveAppointmentByClinicCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("CancelByPatient")]
    public async Task<IActionResult> CancelRequestByPatient
        ([FromBody] [Required] CancelAppointmentByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("ApproveCancellationByPatient")]
    public async Task<IActionResult> CancelRequestByPatient
        ([FromBody] [Required] ApproveAppointmentCancellationByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("CancelAppointmentByClinic")]
    public async Task<IActionResult> CancelAppointmentByClinic(
        [FromBody] [Required] CancelAppointmentByClinicCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("RescheduleAppointment")]
    public async Task<IActionResult> CancelAppointmentByClinic(
        [FromBody] [Required] RescheduleAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}