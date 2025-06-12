using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.ValidateSignIn;

public sealed class DoctorValidateSignInRequestValidator : AbstractValidator<DoctorValidateSignInRequest>
{
    public DoctorValidateSignInRequestValidator()
    {
        RuleFor(r => r.Email).EmailAddress();
        RuleFor(r => r.Otp).Otp();
    }
}