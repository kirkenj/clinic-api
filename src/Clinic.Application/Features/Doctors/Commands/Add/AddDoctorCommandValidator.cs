using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Doctors.Commands.Add;
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