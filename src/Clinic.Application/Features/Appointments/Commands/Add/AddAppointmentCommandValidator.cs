using Clinic.Domain.Abstractions;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.Add;

public sealed class AddAppointmentCommandValidator 
    : AbstractValidator<AddAppointmentCommand>
{
    public AddAppointmentCommandValidator(IDateTimeService dateTimeService)
    {
        RuleFor(a => a.DateTime)
            .NotEmpty()
            .GreaterThan(dateTimeService.UtcNow());
        RuleFor(a => a.DoctorId)
            .NotEmpty();
        RuleFor(a => a.PatientId)
            .NotEmpty();
        RuleFor(a => a.ServiceId)
            .NotEmpty();
    }
}