using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Specializations.Commands.Update;
public sealed class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(m => m.Name)
            .TitleField();
    }
}