using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.SignIn;

public class SendDoctorSignInOtpCommandValidator : AbstractValidator<SendDoctorSignInOtpCommand>
{
    public SendDoctorSignInOtpCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}