using FluentValidation;

namespace Clinic.Application.Features.Office.Commands.UpdateOfficeCommand;
public sealed class UpdateOfficeCommandValidator : AbstractValidator<UpdateOfficeCommand>
{
    public UpdateOfficeCommandValidator()
    {
        RuleFor(c => c.Address)
            .NotEmpty()
            .MinimumLength(10);


        RuleFor(m => m.RegistryPhoneNumber)
          .NotEmpty()
          .Matches("^(\\+\\d{1,2}\\s)?\\(?\\d{3}\\)?[\\s.-]\\d{3}[\\s.-]\\d{4}$")
            .WithMessage("'{PropertyName}' is not a valid phone number. Example: 123-456-7890");
    }
}