using FluentValidation;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;

public class GetPaginatedOfficesRequestValidator : AbstractValidator<GetPaginatedOfficesRequest>
{
    public GetPaginatedOfficesRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}