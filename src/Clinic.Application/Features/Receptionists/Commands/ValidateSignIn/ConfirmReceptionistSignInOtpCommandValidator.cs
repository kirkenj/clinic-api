using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;

public sealed class ConfirmReceptionistSignInOtpCommandValidator : AbstractValidator<ConfirmReceptionistSignInOtpCommand>
{
    public ConfirmReceptionistSignInOtpCommandValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}