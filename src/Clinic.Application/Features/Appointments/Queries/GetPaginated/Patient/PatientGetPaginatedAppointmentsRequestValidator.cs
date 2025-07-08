using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Patient;

public class PatientGetPaginatedAppointmentsRequestValidator : AbstractValidator<PatientGetPaginatedAppointmentsRequest>
{
    public PatientGetPaginatedAppointmentsRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}