using Clinic.Application.Features.Patients.Commands.Add;
using Clinic.Application.Features.Patients.Commands.SignIn;
using Clinic.Application.Features.Patients.Commands.ValidateSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class PatientAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("SignIn")]
    public async Task<ActionResult<string>> SignInAsync([Required] [FromBody] PatientSignInRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("ValidateSignInOtpAsync")]
    public async Task<ActionResult<string>> ValidateSignInOtpAsync(
        [Required] [FromBody] PatientValidateSignUpRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("SignUp")]
    public async Task<ActionResult<string>> ValidateSignInOtpAsync(
        [Required] [FromBody] AddPatientCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Patient")]
    [HttpGet("check-access")]
    public ActionResult<string> CheckToken()
    {
        return NoContent();
    }
}