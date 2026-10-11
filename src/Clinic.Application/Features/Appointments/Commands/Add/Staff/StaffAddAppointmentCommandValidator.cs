using Clinic.Domain.Abstractions;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.Add.Staff;

public sealed class StaffAddAppointmentCommandValidator 
    : AbstractValidator<StaffAddAppointmentCommand>
{
    public StaffAddAppointmentCommandValidator(IDateTimeProvider dateTimeProvider)
    {
        RuleFor(a => a.DateTime)
            .NotEmpty()
            .GreaterThan(dateTimeProvider.UtcNow());
        RuleFor(a => a.DoctorId)
            .NotEmpty();
        RuleFor(a => a.PatientId)
            .NotEmpty();
        RuleFor(a => a.ServiceId)
            .NotEmpty();
    }
}