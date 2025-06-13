using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Receptionists.Commands.SignIn;
using Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Clinic.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public sealed class ReceptionistAuthController(IMediator mediator) : ControllerBase
{
    [HttpPost("send-sign-in-code")]
    public async Task<ActionResult<string>> SendSignInOtpAsync(
        [Required] [FromBody] SendReceptionistSignInOtpCommand request)
    {
        var result = await mediator.Send(request, HttpContext.RequestAborted);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpPost("confirm-sign-in-code")]
    public async Task<ActionResult<string>> ConfirmSignInOtpAsync(
        [Required] [FromBody] ConfirmReceptionistSignInOtpCommand request)
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