using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated;

public class GetPaginatedAppointmentsRequestValidator : AbstractValidator<GetPaginatedAppointmentsRequest>
{
    public GetPaginatedAppointmentsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}