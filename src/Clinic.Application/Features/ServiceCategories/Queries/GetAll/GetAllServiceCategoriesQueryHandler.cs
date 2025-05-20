using Clinic.Application.Responses.ServiceCategories;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Queries.GetAll;

public sealed class GetAllServiceCategoriesQueryHandler(IServiceCategoryRepository serviceCategoryRepository)
    : IRequestHandler<GetAllServiceCategoriesQuery, Result<IEnumerable<ServiceCategoryResponse>>>
{
    public async Task<Result<IEnumerable<ServiceCategoryResponse>>> Handle(GetAllServiceCategoriesQuery request,
        CancellationToken cancellationToken)
    {
        var serviceCategories = await serviceCategoryRepository.GetAllAsync(cancellationToken);
        return Result.Success(ServiceCategoryResponse.FromServiceCategoryCollection(serviceCategories));
    }
}