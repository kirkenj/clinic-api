using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.CancelAppointmentByPatient;

public sealed class CancelAppointmentByPatientCommandValidator : AbstractValidator<CancelAppointmentByPatientCommand>
{
    public CancelAppointmentByPatientCommandValidator()
    {
        RuleFor(a => a.AppointmentId).NotEmpty();
    }
}