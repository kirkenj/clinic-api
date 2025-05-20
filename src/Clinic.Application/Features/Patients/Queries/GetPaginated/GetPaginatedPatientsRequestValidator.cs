using FluentValidation;

namespace Clinic.Application.Features.Patients.Queries.GetPaginated;

public class GetPaginatedPatientsRequestValidator : AbstractValidator<GetPaginatedPatientsRequest>
{
    public GetPaginatedPatientsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}