using FluentValidation;

namespace Clinic.Application.Features.Specialization.Commands.Update;
public sealed class UpdateSpecializationCommandValidator : AbstractValidator<UpdateSpecializationCommand>
{
    public UpdateSpecializationCommandValidator()
    {
        RuleFor(m => m.Name)
            .NotEmpty();
    }
}