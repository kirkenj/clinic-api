using FluentValidation;

namespace Clinic.Application.Features.Specializations.Queries.GetPaginated;

public class GetPaginatedSpecializationsRequestValidator : AbstractValidator<GetPaginatedSpecializationsRequest>
{
    public GetPaginatedSpecializationsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}