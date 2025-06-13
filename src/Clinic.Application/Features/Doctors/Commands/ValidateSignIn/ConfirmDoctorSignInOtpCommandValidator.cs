using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.ValidateSignIn;

public sealed class ConfirmDoctorSignInOtpCommandValidator : AbstractValidator<ConfirmDoctorSignInOtpCommand>
{
    public ConfirmDoctorSignInOtpCommandValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}