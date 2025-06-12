using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.ValidateSignIn;

public sealed class PatientValidateSignUpRequestValidator : AbstractValidator<PatientValidateSignUpRequest>
{
    public PatientValidateSignUpRequestValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}