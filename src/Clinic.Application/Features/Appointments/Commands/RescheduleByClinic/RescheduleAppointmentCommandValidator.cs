using Clinic.Domain.Abstractions;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;

public sealed class RescheduleAppointmentCommandValidator
    : AbstractValidator<RescheduleAppointmentCommand>
{
    public RescheduleAppointmentCommandValidator(IDateTimeService dateTimeService)
    {
        RuleFor(c => c.AppointmentId)
            .NotEmpty();
        RuleFor(c => c.NewDateTime)
            .GreaterThanOrEqualTo(dateTimeService.UtcNow());
        RuleFor(c => c.NewDoctorId)
            .NotEmpty();
    }
}