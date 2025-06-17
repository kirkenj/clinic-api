using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignUp;

public sealed class ConfirmPatientSignUpCommandValidator : AbstractValidator<ConfirmPatientSignUpCommand>
{
    public ConfirmPatientSignUpCommandValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}