using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Offices.Commands.Delete;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureApi.Functions.Office;

public class DeleteOffice(IMediator mediator)
{
    [Function("DeleteOffice")]
    public async Task<IActionResult> DeleteOfficeAsync(
        [HttpTrigger(AuthorizationLevel.Function, "delete", Route = "office/{id}")]
        HttpRequest req,
        [FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new DeleteOfficeCommand { Id = id }, req.HttpContext.RequestAborted);
        return result.IsSuccess ? new NoContentResult() : new BadRequestObjectResult(result.Error);
    }
}