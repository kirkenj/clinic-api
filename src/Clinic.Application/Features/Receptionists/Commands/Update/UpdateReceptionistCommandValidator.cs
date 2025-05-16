using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Receptionists.Commands.Update;

public sealed class UpdateReceptionistCommandValidator : AbstractValidator<UpdateReceptionistCommand>
{
    public UpdateReceptionistCommandValidator()
    {
        RuleFor(c => c.FirstName)
            .TitleField();

        RuleFor(c => c.LastName)
            .TitleField();

        RuleFor(c => c.MiddleName)
            .TitleField();

        RuleFor(c => c.PhoneNumber)
            .BelarusianPhoneNumber();

        RuleFor(m => m.Email)
            .EmailAddress();
    }
}