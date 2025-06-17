using FluentValidation;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;

public class GetPaginatedAppointmentResultsRequestValidator : AbstractValidator<GetPaginatedAppointmentResultsRequest>
{
    public GetPaginatedAppointmentResultsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}