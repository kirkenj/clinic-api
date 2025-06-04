using FluentValidation;

namespace Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;

public sealed class SendOtpToCreateAppointmentRequestValidator : AbstractValidator<SendOtpToCreateAppointmentRequest>
{
    public SendOtpToCreateAppointmentRequestValidator()
    {
        RuleFor(a => a.AppointmentId)
            .NotEmpty();
    }
}