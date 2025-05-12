using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Offices.Commands.Add;
public sealed class AddOfficeCommandValidator : AbstractValidator<AddOfficeCommand>
{
    public AddOfficeCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Address)
            .Address();
    }
}