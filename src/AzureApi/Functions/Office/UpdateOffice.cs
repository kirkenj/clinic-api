using System.Text.Json;
using Clinic.Application.Features.Offices.Commands.Update;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureApi.Functions.Office;

public class UpdateOffice(IMediator mediator)
{
    [Function("UpdateOffice")]
    public async Task<IActionResult> UpdateOfficeAsync(
        [HttpTrigger(AuthorizationLevel.Function, "put", Route = "office")]
        HttpRequest req)
    {
        var bodyString = await new StreamReader(req.Body).ReadToEndAsync(req.HttpContext.RequestAborted);
        var request = JsonSerializer.Deserialize<UpdateOfficeCommand>(bodyString, JsonSerializerOptions.Web);
        if (request is null)
        {
            return new BadRequestResult();
        }

        var result = await mediator.Send(request, req.HttpContext.RequestAborted);
        return result.IsSuccess ? new NoContentResult() : new BadRequestObjectResult(result.Error);
    }
}