using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Services.Commands.Add;
using Clinic.Application.Features.Services.Commands.Delete;
using Clinic.Application.Features.Services.Commands.Update;
using Clinic.Application.Features.Services.Queries.Get;
using Clinic.Application.Features.Services.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ServiceController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedServicesAsync([FromQuery] ServiceFilter? serviceFilter, 
        [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedServicesRequest 
                { ServiceFilter = serviceFilter, Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetServiceQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new DeleteServiceCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPost]
    public async Task<IActionResult> AddServiceAsync([FromBody][Required] AddServiceCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPut]
    public async Task<IActionResult> UpdateServiceAsync(
        [FromBody][Required] UpdateServiceCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}