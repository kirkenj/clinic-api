using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.SignIn;

public class SendPatientSignInOtpCommandValidator : AbstractValidator<SendPatientSignInOtpCommand>
{
    public SendPatientSignInOtpCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}