using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.SignUp;

public sealed class SendPatientSignUpOtpCommandValidator : AbstractValidator<SendPatientSignUpOtpCommand>
{
    public SendPatientSignUpOtpCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .TitleField();
        RuleFor(c => c.LastName)
            .TitleField();
        RuleFor(c => c.MiddleName)
            .TitleField();
        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();
        RuleFor(m => m.Email)
            .EmailAddress();
    }
}