using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public sealed class ApproveAppointmentCancellationByPatientCommandValidator
    : AbstractValidator<ApproveAppointmentCancellationByPatientCommand>
{
    public ApproveAppointmentCancellationByPatientCommandValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();
        RuleFor(a => a.Otp)
            .Otp();
    }
}