using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Queries.Get;

public sealed class GetServiceCategoryQueryHandler(IServiceCategoryRepository serviceCategoryRepository)
    : IRequestHandler<GetServiceCategoryQuery, Result<ServiceCategoryResponse>>
{
    public async Task<Result<ServiceCategoryResponse>> Handle(GetServiceCategoryQuery request,
        CancellationToken cancellationToken)
    {
        var serviceCategory = await serviceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (serviceCategory is null)
        {
            return Result.Failure<ServiceCategoryResponse>(
                ServiceCategoryErrors.NotFound(request.Id));
        }

        return ServiceCategoryResponse.FromServiceCategory(serviceCategory);
    }
}