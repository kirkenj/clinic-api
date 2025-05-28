using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.Add;

public sealed class AddAppointmentCommandValidator : AbstractValidator<AddAppointmentCommand>
{
    public AddAppointmentCommandValidator()
    {
        RuleFor(a => a.DateTime)
            .NotEmpty();
        RuleFor(a => a.DoctorId)
            .NotEmpty();
        RuleFor(a => a.PatientId)
            .NotEmpty();
        RuleFor(a => a.ServiceId)
            .NotEmpty();
    }
}