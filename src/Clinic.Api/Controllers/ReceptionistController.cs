using Clinic.Application.Features.Receptionists.Queries.Get;
using Clinic.Application.Features.Receptionists.Queries.GetPaginated;
using Clinic.Domain.Filters;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReceptionistController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedReceptionistsAsync([FromQuery] ReceptionistFilter? receptionistFilter,
        [FromQuery][Required] int page, [FromQuery][Required] int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedReceptionistsRequest
        {
            ReceptionistFilter = receptionistFilter,
            Page = page,
            PageSize = pageSize
        },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReceptionistAsync([FromRoute][Required] uint id)
    {
        var result = await mediator.Send(new GetReceptionistQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}