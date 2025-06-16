using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Doctors.Commands.SignIn;
using Clinic.Application.Features.Doctors.Commands.ValidateSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class DoctorAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("send-sign-in-code")]
    public async Task<ActionResult<string>> SendSignInOtpAsync(
        [Required] [FromBody] SendDoctorSignInOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("confirm-sign-in-code")]
    public async Task<ActionResult<string>> ConfirmSignInOtpAsync(
        [Required] [FromBody] ConfirmDoctorSignInOtpCommand request)
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