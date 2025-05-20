using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.ServiceCategories.Commands.Update;

public sealed class UpdateServiceCategoryCommandValidator : AbstractValidator<UpdateServiceCategoryCommand>
{
    public UpdateServiceCategoryCommandValidator()
    {
        RuleFor(c => c.Name)
            .TitleField();
        RuleFor(m => m.Duration)
            .NotEmpty();
    }
}