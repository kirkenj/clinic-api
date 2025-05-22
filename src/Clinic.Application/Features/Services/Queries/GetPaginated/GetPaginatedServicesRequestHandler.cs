using Clinic.Application.Extensions;
using Clinic.Application.Responses.Services;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Services.Queries.GetPaginated;

public class GetPaginatedServicesRequestHandler
    (ILogger<GetPaginatedServicesRequestHandler> logger,
    IServiceRepository serviceRepository,
    GetPaginatedServicesRequestValidator validator) : IRequestHandler<GetPaginatedServicesRequest,
    Result<PaginatedCollection<ServiceResponse>>>
{
    public async Task<Result<PaginatedCollection<ServiceResponse>>> Handle(GetPaginatedServicesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedServicesRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<ServiceResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedServices =
            await serviceRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var serviceResponse = paginatedServices.Select(ServiceResponse.FromService);
        logger.LogInformation("PaginatedServicesResponse: {@response}", serviceResponse);
        return Result.Success(serviceResponse);
    }
}