using FluentValidation;

namespace Clinic.Application.Features.Specialization.Commands.Add;
public sealed class AddSpecializationCommandValidator : AbstractValidator<AddSpecializationCommand>
{
    public AddSpecializationCommandValidator()
    {
        RuleFor(c => c.Name)
            .NotEmpty();
    }
}