using Clinic.Application.Features.Specializations.Commands.Add;
using Clinic.Application.Features.Specializations.Commands.Delete;
using Clinic.Application.Features.Specializations.Commands.Update;
using Clinic.Application.Features.Specializations.Queries.Get;
using Clinic.Application.Features.Specializations.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class SpecializationController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedSpecializationsAsync([FromQuery] SpecializationFilter? filter,
        [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(
            new GetPaginatedSpecializationsRequest { Filter = filter, Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecializationAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new GetSpecializationQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecializationAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new DeleteSpecializationCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPost]
    public async Task<IActionResult> AddSpecializationAsync([FromBody][Required] AddSpecializationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpPut]
    public async Task<IActionResult> UpdateSpecializationAsync(
        [FromBody][Required] UpdateSpecializationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}