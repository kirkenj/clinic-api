using Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;
using Clinic.Application.Features.Appointments.Commands.Add.Patient;
using Clinic.Application.Features.Appointments.Commands.Add.Staff;
using Clinic.Application.Features.Appointments.Commands.ApproveByClinic;
using Clinic.Application.Features.Appointments.Commands.ApproveByPatient;
using Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;
using Clinic.Application.Features.Appointments.Commands.CancelByClinic;
using Clinic.Application.Features.Appointments.Commands.CancelByPatient;
using Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;
using Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;
using Clinic.Application.Features.Appointments.Queries.Get.Staff;
using Clinic.Application.Features.Appointments.Queries.GetAvailableSlots.cs;
using Clinic.Application.Features.Appointments.Queries.GetPaginated.Patient;
using Clinic.Application.Features.Appointments.Queries.GetPaginated.Staff;
using Clinic.Application.Options;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AppointmentController
    (IMediator mediator,
    IOptions<AppointmentsOptions> options)
    : ControllerBase
{
    [HttpGet("available-slots-for-doctor")]
    public async Task<IActionResult> AvailableSlotsForDoctor([FromQuery][Required] GetDoctorsAvailableSlotsOnDateQuery request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedAppointmentsAsync([FromQuery] AppointmentFilter? appointmentFilter,
    [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new StaffGetPaginatedAppointmentsRequest
        {
            AppointmentFilter = appointmentFilter,
            Page = page,
            PageSize = pageSize
        },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new GetAppointmentQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPost]
    public async Task<IActionResult> AddAppointmentAsync([FromBody][Required] StaffAddAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles ="Patient")]
    [HttpPost("as-patient")]
    public async Task<IActionResult>AddAppointment(PatientAddAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("as-patient")]
    public async Task<IActionResult> GetPaginatedAppointmentsAsPatientAsync([FromQuery] PatientAppointmentFilter? filter,
        [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new PatientGetPaginatedAppointmentsRequest
        {
            AppointmentFilter = filter,
            Page = page,
            PageSize = pageSize
        },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("as-patient/{id}")]
    public async Task<IActionResult> GetAppointmentAsPatientAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new GetAppointmentQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Patient")]
    [HttpPost("send-otp-to-confirm-appointment-by-patient")]
    public async Task<IActionResult> SendOtpToConfirmAppointment(SendOtpToCreateAppointmentRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Patient")]
    [HttpPost("approve-by-patient")]
    public async Task<IActionResult> ApproveRequestByPatient
        ([FromBody][Required] ApproveAppointmentByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpPost("approve-by-clinic")]
    public async Task<IActionResult> ApproveRequestByClinic
        ([FromBody][Required] ApproveAppointmentByClinicCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Patient")]
    [HttpPost("cancel-by-patient")]
    public async Task<IActionResult> CancelRequestByPatient
        ([FromBody][Required] CancelAppointmentByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Patient")]
    [HttpPost("approve-cancellation-by-patient")]
    public async Task<IActionResult> ApproveCancelationByPatient
        ([FromBody][Required] ApproveAppointmentCancellationByPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpPost("cancel-appointment-by-clinic")]
    public async Task<IActionResult> CancelAppointmentByClinic(
        [FromBody][Required] CancelAppointmentByClinicCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpPost("reschedule")]
    public async Task<IActionResult> CancelAppointmentByClinic(
        [FromBody][Required] RescheduleAppointmentCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpGet("timetable-options")]
    public IActionResult GetSlotsOptions()
    {
        return Ok(options.Value);
    }
}