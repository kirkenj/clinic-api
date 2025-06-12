using Clinic.Application.Features.Doctors.Commands.SignIn;
using Clinic.Application.Features.Doctors.Commands.ValidateSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class DoctorAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("SignIn")]
    public async Task<ActionResult<string>> SignInAsync([Required] [FromBody] DoctorSignInRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("ValidateSignInOtpAsync")]
    public async Task<ActionResult<string>> ValidateSignInOtpAsync(
        [Required] [FromBody] DoctorValidateSignInRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Doctor")]
    [HttpGet("check-access")]
    public ActionResult<string> CheckToken()
    {
        return NoContent();
    }
}