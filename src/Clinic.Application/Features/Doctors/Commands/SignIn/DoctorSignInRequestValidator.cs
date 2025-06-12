using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.SignIn;

public class DoctorSignInRequestValidator : AbstractValidator<DoctorSignInRequest>
{
    public DoctorSignInRequestValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}