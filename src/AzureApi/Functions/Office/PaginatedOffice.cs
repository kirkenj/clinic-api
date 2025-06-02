using Clinic.Application.Features.Offices.Queries.GetPaginated;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;

namespace AzureApi.Functions.Office;

public class PaginatedOffice(IMediator mediator)
{
    [Function("PaginatedOffice")]
    public async Task<IActionResult> Run([HttpTrigger(AuthorizationLevel.Function, "get", Route = "office/page")] HttpRequest req,
        int page, int pageSize)
    {
        var result = await mediator.Send(new GetPaginatedOfficesRequest { Page = page, PageSize = pageSize },
            req.HttpContext.RequestAborted);
        return result.IsSuccess ? new OkObjectResult(result.Value) : new BadRequestObjectResult(result.Error);
    }
}