using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Staff;

public class StaffGetPaginatedAppointmentsRequestValidator : AbstractValidator<StaffGetPaginatedAppointmentsRequest>
{
    public StaffGetPaginatedAppointmentsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}