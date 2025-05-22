using FluentValidation;

namespace Clinic.Application.Features.Services.Queries.GetPaginated;

public class GetPaginatedServicesRequestValidator : AbstractValidator<GetPaginatedServicesRequest>
{
    public GetPaginatedServicesRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}