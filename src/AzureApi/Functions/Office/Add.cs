using System.Text.Json;
using Clinic.Application.Features.Offices.Commands.Add;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureApi.Functions.Office;

public class Add(IMediator mediator)
{
    [Function("office_add")]
    public async Task<IActionResult> AddOfficeAsync(
        [HttpTrigger(AuthorizationLevel.Function, "post", Route = "office")]
        HttpRequest req)
    {
        var bodyString = await new StreamReader(req.Body).ReadToEndAsync(req.HttpContext.RequestAborted);
        var request = JsonSerializer.Deserialize<AddOfficeCommand>(bodyString, JsonSerializerOptions.Web);
        if (request is null)
        {
            return new BadRequestResult();
        }

        var result = await mediator.Send(request, req.HttpContext.RequestAborted);
        return result.IsSuccess ? new NoContentResult() : new BadRequestObjectResult(result.Error);
    }
}