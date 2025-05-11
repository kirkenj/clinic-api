using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Doctor.Commands.Add;
public sealed class AddDoctorCommandValidator : AbstractValidator<AddDoctorCommand>
{
    public AddDoctorCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Email)
            .EmailAddress();
    }
}