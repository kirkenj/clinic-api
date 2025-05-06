using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Office.Commands.AddOfficeCommand;
public sealed class AddOfficeCommandValidator : AbstractValidator<AddOfficeCommand>
{
    public AddOfficeCommandValidator()
    {
        RuleFor(c => c.RegistryPhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Address)
            .Address();
    }
}