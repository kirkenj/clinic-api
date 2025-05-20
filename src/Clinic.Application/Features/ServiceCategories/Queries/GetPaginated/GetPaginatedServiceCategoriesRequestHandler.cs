using Clinic.Application.Extensions;
using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.ServiceCategories.Queries.GetPaginated;

public class GetPaginatedServiceCategoriesRequestHandler
(ILogger<GetPaginatedServiceCategoriesRequestHandler> logger,
    IServiceCategoryRepository serviceCategoryRepository,
    GetPaginatedServiceCategoriesRequestValidator validator) : IRequestHandler<GetPaginatedServiceCategoriesRequest,
    Result<PaginatedCollection<ServiceCategoryResponse>>>
{
    public async Task<Result<PaginatedCollection<ServiceCategoryResponse>>> Handle(
        GetPaginatedServiceCategoriesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedServiceCategoriesRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<ServiceCategoryResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedServiceCategories =
            await serviceCategoryRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var serviceCategoryResponse = paginatedServiceCategories.Select(ServiceCategoryResponse.FromServiceCategory);
        logger.LogInformation("PaginatedServiceCategoriesResponse: {@response}", serviceCategoryResponse);
        return Result.Success(serviceCategoryResponse);
    }
}