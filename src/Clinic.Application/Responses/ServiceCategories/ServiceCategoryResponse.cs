using Clinic.Domain.Entities;

namespace Clinic.Application.Responses.ServiceCategories;

public sealed record ServiceCategoryResponse(
    uint Id,
    string Name,
    TimeSpan Duration,
    bool IsActive
)
{
    public static ServiceCategoryResponse FromServiceCategory(ServiceCategory serviceCategory)
    {
        return new ServiceCategoryResponse(
            serviceCategory.Id,
            serviceCategory.Name,
            serviceCategory.Duration,
            serviceCategory.IsActive
        );
    }

    public static IEnumerable<ServiceCategoryResponse> FromServiceCategoryCollection(
        IEnumerable<ServiceCategory> serviceCategories)
    {
        return serviceCategories.Select(FromServiceCategory);
    }
}