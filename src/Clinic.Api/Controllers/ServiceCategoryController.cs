using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.ServiceCategories.Commands.Add;
using Clinic.Application.Features.ServiceCategories.Commands.Delete;
using Clinic.Application.Features.ServiceCategories.Commands.Update;
using Clinic.Application.Features.ServiceCategories.Queries.Get;
using Clinic.Application.Features.ServiceCategories.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ServiceCategoryController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedServiceCategoriesAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedServiceCategoriesRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetServiceCategoryAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetServiceCategoryQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteServiceCategoryAsync([FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new DeleteServiceCategoryCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddServiceCategoryAsync([FromBody] [Required] AddServiceCategoryCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateServiceCategoryAsync(
        [FromBody] [Required] UpdateServiceCategoryCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}