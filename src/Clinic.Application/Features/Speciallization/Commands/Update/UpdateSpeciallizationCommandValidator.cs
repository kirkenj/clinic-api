using FluentValidation;

namespace Clinic.Application.Features.Speciallization.Commands.Update;
public sealed class UpdateSpeciallizationCommandValidator : AbstractValidator<UpdateSpeciallizationCommand>
{
    public UpdateSpeciallizationCommandValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty();
    }
}