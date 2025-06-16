using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Patients.Commands.ConfirmSignIn;
using Clinic.Application.Features.Patients.Commands.ConfirmSignUp;
using Clinic.Application.Features.Patients.Commands.ResendSignUpOtp;
using Clinic.Application.Features.Patients.Commands.SignIn;
using Clinic.Application.Features.Patients.Commands.SignUp;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PatientAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("send-sign-in-code")]
    public async Task<ActionResult<string>> SendSignInOtpAsync(
        [Required] [FromBody] SendPatientSignInOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("confirm-sign-in-code")]
    public async Task<ActionResult<string>> ConfirmSignInOtpAsync(
        [Required] [FromBody] ConfirmPatientSignInOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("sign-up")]
    public async Task<ActionResult<string>> SignUpAsync(
        [Required] [FromBody] SendPatientSignUpOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("send-sign-up-code")]
    public async Task<ActionResult<string>> SendSignUpOtpAsync(
        [Required] [FromBody] ResendPatientSignUpOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [HttpPost("confirm-sign-up-code")]
    public async Task<ActionResult<string>> ConfirmSignUpOtpAsync(
        [Required] [FromBody] ConfirmPatientSignUpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? NoContent() : BadRequest(result.Error);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("check-access")]
    public ActionResult<string> CheckToken()
    {
        return NoContent();
    }
}