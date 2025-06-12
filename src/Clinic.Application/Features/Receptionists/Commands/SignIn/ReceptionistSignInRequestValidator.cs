using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Commands.SignIn;

public class ReceptionistSignInRequestValidator : AbstractValidator<ReceptionistSignInRequest>
{
    public ReceptionistSignInRequestValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}