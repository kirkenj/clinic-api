using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByPatient;

public sealed class
    ApproveAppointmentByPatientCommandValidator : AbstractValidator<ApproveAppointmentByPatientCommand>
{
    public ApproveAppointmentByPatientCommandValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();

        RuleFor(a => a.Otp)
            .Otp();
    }
}