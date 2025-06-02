using System.ComponentModel.DataAnnotations;
using Clinic.Application.Features.Offices.Queries.Get;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureApi.Functions.Office;

public class Get(IMediator mediator)
{
    [Function("office_one")]
    public async Task<IActionResult> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "office/{id}")]
        HttpRequest req,
        [FromRoute] [Required] uint id)
    {
        var result = await mediator.Send(new GetOfficeQuery { Id = id }, req.HttpContext.RequestAborted);
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(result.Error);
    }
}