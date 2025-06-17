using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.AppointmentResults.Commands.Add;
using Clinic.Application.Features.AppointmentResults.Commands.Delete;
using Clinic.Application.Features.AppointmentResults.Commands.Update;
using Clinic.Application.Features.AppointmentResults.Queries.Get;
using Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class AppointmentResultController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedAppointmentResultsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedAppointmentResultsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetAppointmentResultAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetAppointmentResultQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteAppointmentResultAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new DeleteAppointmentResultCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddAppointmentResultAsync([FromBody] [Required] AddAppointmentResultCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateAppointmentResultAsync([FromBody] [Required] UpdateAppointmentResultCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}