using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandValidator : AbstractValidator<UpdateAppointmentCommand>
{
    public UpdateAppointmentCommandValidator()
    {
        RuleFor(a => a.Id)
            .NotEmpty();
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