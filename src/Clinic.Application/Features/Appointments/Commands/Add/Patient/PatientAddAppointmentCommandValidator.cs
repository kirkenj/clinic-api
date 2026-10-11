using Clinic.Domain.Abstractions;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.Add.Patient;

public sealed class PatientAddAppointmentCommandValidator
    : AbstractValidator<PatientAddAppointmentCommand>
{
    public PatientAddAppointmentCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(a => a.DateTime)
            .NotEmpty()
            .GreaterThan(dateTimeProvider.UtcNow());
        RuleFor(a => a.DoctorId)
            .NotEmpty();
        RuleFor(a => a.ServiceId)
            .NotEmpty();
    }
}