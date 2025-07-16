using Clinic.Application.Features.AppointmentResults.Commands.Add;
using Clinic.Application.Features.AppointmentResults.Commands.Delete;
using Clinic.Application.Features.AppointmentResults.Commands.Update;
using Clinic.Application.Features.AppointmentResults.Queries.GetByAppointmentId;
using Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AppointmentResultController(IMediator mediator) : ControllerBase
{
    [Authorize(Roles = "Receptionist,Doctor,Patient")]
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedAppointmentResultsAsync([FromQuery] AppointmentResultFilter? appointmentResultFilter,
    [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedAppointmentResultsRequest
        {
            AppointmentResultFilter = appointmentResultFilter,
            Page = page,
            PageSize = pageSize
        },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor,Patient")]
    [HttpGet("{appointmentId}")]
    public async Task<IActionResult> GetAppointmentResultAsync([FromRoute][Required] uint appointmentId)
    {
        var result = await mediator.Send(new GetAppointmentResultByAppointmentIdQuery { AppointmentId = appointmentId }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointmentResultAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new DeleteAppointmentResultCommand { AppointmentId = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpPost]
    public async Task<IActionResult> AddAppointmentResultAsync([FromBody][Required] AddAppointmentResultCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist,Doctor")]
    [HttpPut]
    public async Task<IActionResult> UpdateAppointmentResultAsync([FromBody][Required] UpdateAppointmentResultCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}