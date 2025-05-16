using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Specializations.Commands.Add;

public sealed class AddSpecializationCommandValidator : AbstractValidator<AddSpecializationCommand>
{
    public AddSpecializationCommandValidator()
    {
        RuleFor(c => c.Name)
            .TitleField();
    }
}