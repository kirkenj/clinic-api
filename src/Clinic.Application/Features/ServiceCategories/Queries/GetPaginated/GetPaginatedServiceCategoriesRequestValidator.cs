using FluentValidation;

namespace Clinic.Application.Features.ServiceCategories.Queries.GetPaginated;

public class GetPaginatedServiceCategoriesRequestValidator : AbstractValidator<GetPaginatedServiceCategoriesRequest>
{
    public GetPaginatedServiceCategoriesRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}