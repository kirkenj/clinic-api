using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByPatient;

public sealed class
    SetApprovedAppointmentByPatientCommandValidator : AbstractValidator<SetApprovedAppointmentByPatientCommand>
{
    public SetApprovedAppointmentByPatientCommandValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();
    }
}