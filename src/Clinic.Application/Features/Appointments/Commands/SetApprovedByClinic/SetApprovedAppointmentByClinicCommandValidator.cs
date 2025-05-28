using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByClinic;

public sealed class
    SetApprovedAppointmentByClinicCommandValidator : AbstractValidator<SetApprovedAppointmentByClinicCommand>
{
    public SetApprovedAppointmentByClinicCommandValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();
    }
}