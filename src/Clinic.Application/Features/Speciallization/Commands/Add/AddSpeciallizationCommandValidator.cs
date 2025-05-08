using FluentValidation;

namespace Clinic.Application.Features.Speciallization.Commands.Add;
public sealed class AddSpeciallizationCommandValidator : AbstractValidator<AddSpeciallizationCommand>
{
    public AddSpeciallizationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty();
    }
}