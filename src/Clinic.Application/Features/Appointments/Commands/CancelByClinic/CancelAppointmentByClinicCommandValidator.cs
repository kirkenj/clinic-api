using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.CancelByClinic;

public sealed class CancelAppointmentByClinicCommandValidator : AbstractValidator<CancelAppointmentByClinicCommand>
{
    public CancelAppointmentByClinicCommandValidator()
    {
        RuleFor(a => a.Id).NotEmpty();
    }
}