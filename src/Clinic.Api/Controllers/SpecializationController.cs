using Clinic.Application.Features.Specializations.Commands.Add;
using Clinic.Application.Features.Specializations.Commands.Delete;
using Clinic.Application.Features.Specializations.Commands.Update;
using Clinic.Application.Features.Specializations.Queries.Get;
using Clinic.Application.Features.Specializations.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class SpecializationController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedSpecializationsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedSpecializationsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSpecializationAsync(uint id)
    {
        var result = await mediator.Send(new GetSpecializationQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSpecializationAsync(uint id)
    {
        var result = await mediator.Send(new DeleteSpecializationCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddSpecializationAsync(AddSpecializationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSpecializationAsync(UpdateSpecializationCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}