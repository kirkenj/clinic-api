using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;

public sealed class RescheduleAppointmentCommandValidator
    : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator()
    {
        RuleFor(c => c.AppointmentId)
            .NotEmpty();
        RuleFor(c => c.NewDateTime)
            .GreaterThanOrEqualTo(DateTime.UtcNow);
        RuleFor(c => c.NewDoctorId)
            .NotEmpty();
    }
}