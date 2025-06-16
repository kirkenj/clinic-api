using FluentValidation;

namespace Clinic.Application.Features.Patients.Commands.ResendSignUpOtp;

public sealed class ResendPatientSignUpOtpCommandValidator : AbstractValidator<ResendPatientSignUpOtpCommand>
{
    public ResendPatientSignUpOtpCommandValidator()
    {
        RuleFor(c => c.Email).EmailAddress();
    }
}