using Clinic.Application.Features.Doctors.Commands.Add;
using Clinic.Application.Features.Doctors.Commands.Delete;
using Clinic.Application.Features.Doctors.Commands.Update;
using Clinic.Application.Features.Doctors.Queries.Get;
using Clinic.Application.Features.Doctors.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class DoctorController(IMediator mediator) : ControllerBase
{
    [HttpGet("paged")]
    public async Task<IActionResult> GetPaginatedDoctorsAsync(int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedDoctorsRequest { Page = page, PageSize = pageSize },
            HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetDoctorAsync(uint id)
    {
        var result = await mediator.Send(new GetDoctorQuery { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDoctorAsync(uint id)
    {
        var result = await mediator.Send(new DeleteDoctorCommand { Id = id }, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPost]
    public async Task<IActionResult> AddDoctorAsync(AddDoctorCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateDoctorAsync(UpdateDoctorCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok() : BadRequest(result.Error);
    }
}