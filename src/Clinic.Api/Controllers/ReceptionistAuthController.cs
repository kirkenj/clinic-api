using Clinic.Application.Features.Receptionists.Commands.SignIn;
using Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReceptionistAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("SignIn")]
    public async Task<ActionResult<string>> SignInAsync([Required] [FromBody] ReceptionistSignInRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("ValidateSignInOtpAsync")]
    public async Task<ActionResult<string>> ValidateSignInOtpAsync(
        [Required] [FromBody] ReceptionistValidateSignInRequest request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [Authorize(Roles = "Receptionist")]
    [HttpGet("check-access")]
    public ActionResult<string> CheckToken()
    {
        return NoContent();
    }
}