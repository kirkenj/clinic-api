using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginatedForPatient;

public class
    GetPaginatedAppointmentsForPatientRequestValidator : AbstractValidator<GetPaginatedAppointmentsForPatientRequest>
{
    public GetPaginatedAppointmentsForPatientRequestValidator()
    {
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}