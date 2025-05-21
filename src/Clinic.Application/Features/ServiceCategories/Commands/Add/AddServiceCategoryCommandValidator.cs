using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.ServiceCategories.Commands.Add;

public sealed class AddServiceCategoryCommandValidator : AbstractValidator<AddServiceCategoryCommand>
{
    public AddServiceCategoryCommandValidator()
    {
        RuleFor(c => c.Name)
            .TitleField();
        RuleFor(m => m.Duration)
            .NotEmpty();
    }
}