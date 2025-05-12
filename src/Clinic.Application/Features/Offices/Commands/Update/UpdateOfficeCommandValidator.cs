using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Offices.Commands.Update;
public sealed class UpdateOfficeCommandValidator : AbstractValidator<UpdateOfficeCommand>
{
    public UpdateOfficeCommandValidator()
    {
        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Address)
            .Address();
    }
}