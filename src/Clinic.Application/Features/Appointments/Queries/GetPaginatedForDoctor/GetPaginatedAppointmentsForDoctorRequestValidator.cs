using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginatedForDoctor;

public class
    GetPaginatedAppointmentsForDoctorRequestValidator : AbstractValidator<GetPaginatedAppointmentsForDoctorRequest>
{
    public GetPaginatedAppointmentsForDoctorRequestValidator()
    {
        RuleFor(r => r.DoctorId)
            .NotEmpty();
        RuleFor(r => r.Page)
            .GreaterThan(0);
        RuleFor(r => r.PageSize)
            .GreaterThan(0);
    }
}