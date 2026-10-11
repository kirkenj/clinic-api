using Clinic.Application.Extensions;
using Clinic.Application.Responses.Services;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Services.Queries.GetPaginated;

public class GetPaginatedServicesRequestHandler
    (IServiceRepository serviceRepository,
    GetPaginatedServicesRequestValidator validator) 
    : IRequestHandler<GetPaginatedServicesRequest, Result<PaginatedCollection<ServiceResponse>>>
{
    public async Task<Result<PaginatedCollection<ServiceResponse>>> Handle(GetPaginatedServicesRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<ServiceResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        if (request.ServiceFilter is { PriceStart: not null, PriceEnd: not null }
            && request.ServiceFilter.PriceStart.Value > request.ServiceFilter.PriceEnd.Value)
        {
            (request.ServiceFilter.PriceStart, request.ServiceFilter.PriceEnd) =
                (request.ServiceFilter.PriceEnd, request.ServiceFilter.PriceStart);
        }

        var paginatedServices = await serviceRepository.GetPaginatedAsync(request.ServiceFilter, 
            request.Page, request.PageSize, cancellationToken);
        var serviceResponse = paginatedServices.Select(ServiceResponse.FromService);
        return Result.Success(serviceResponse);
    }
}