using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;

public sealed class ReceptionistValidateSignInRequestValidator : AbstractValidator<ReceptionistValidateSignInRequest>
{
    public ReceptionistValidateSignInRequestValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}