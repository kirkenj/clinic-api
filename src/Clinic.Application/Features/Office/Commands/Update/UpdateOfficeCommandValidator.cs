using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Office.Commands.Update;
public sealed class UpdateOfficeCommandValidator : AbstractValidator<UpdateOfficeCommand>
{
    public UpdateOfficeCommandValidator()
    {
        RuleFor(c => c.RegistryPhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Address)
            .Address();
    }
}