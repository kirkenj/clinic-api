using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class SendReceptionistSignInOtpCommandValidator : AbstractValidator<SendReceptionistSignInOtpCommand>
{
    public SendReceptionistSignInOtpCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}