using FluentValidation;

namespace Clinic.Application.Features.Doctors.Queries.GetPaginated;

public class GetPaginatedDoctorsRequestValidator : AbstractValidator<GetPaginatedDoctorsRequest>
{
    public GetPaginatedDoctorsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}