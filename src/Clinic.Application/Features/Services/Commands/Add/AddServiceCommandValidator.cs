using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Services.Commands.Add;

public sealed class AddServiceCommandValidator : AbstractValidator<AddServiceCommand>
{
    public AddServiceCommandValidator()
    {
        RuleFor(m => m.Name)
            .TitleField();
        RuleFor(m => m.Price)
            .Price();
        RuleFor(m => m.SpecializationId)
            .NotEmpty();
        RuleFor(m => m.ServiceCategoryId)
            .NotEmpty();
    }
}