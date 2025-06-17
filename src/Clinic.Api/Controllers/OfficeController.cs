using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Offices.Commands.Add;
using Clinic.Application.Features.Offices.Commands.Delete;
using Clinic.Application.Features.Offices.Commands.Update;
using Clinic.Application.Features.Offices.Queries.Get;
using Clinic.Application.Features.Offices.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class OfficeController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedOfficesAsync([FromQuery] OfficeFilter? officeFilter,
        [FromQuery] [Required] int page, [FromQuery] [Required] int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedOfficesRequest
                { OfficeFilter = officeFilter, Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetOfficeAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetOfficeQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteOfficeAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new DeleteOfficeCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddOfficeAsync([FromBody] [Required] AddOfficeCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateOfficeAsync([FromBody] [Required] UpdateOfficeCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }
}