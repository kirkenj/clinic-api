using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignIn;

public sealed class ConfirmPatientSignInOtpCommandValidator : AbstractValidator<ConfirmPatientSignInOtpCommand>
{
    public ConfirmPatientSignInOtpCommandValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}