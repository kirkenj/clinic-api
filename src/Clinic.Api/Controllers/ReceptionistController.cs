using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Receptionists.Commands.Update;
using Clinic.Application.Features.Receptionists.Queries.Get;
using Clinic.Application.Features.Receptionists.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReceptionistController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedReceptionistsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedReceptionistsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetReceptionistAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetReceptionistQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateReceptionistAsync([FromBody] [Required] UpdateReceptionistCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}