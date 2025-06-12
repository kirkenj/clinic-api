using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.SignIn;

public class PatientSignInRequestValidator : AbstractValidator<PatientSignInRequest>
{
    public PatientSignInRequestValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}