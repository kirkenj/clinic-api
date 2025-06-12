using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.ValidateSignUp;

public sealed class PatientValidateSignUpRequestValidator : AbstractValidator<ValidateSignIn.PatientValidateSignUpRequest>
{
    public PatientValidateSignUpRequestValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}