using Clinic.Application.Validation;
using FluentValidation;

namespace Clinic.Application.Features.Services.Commands.Update;

public sealed class UpdateServiceCommandValidator : AbstractValidator<UpdateServiceCommand>
{
    public UpdateServiceCommandValidator()
    {
        RuleFor(m => m.Id)
            .NotEmpty();
        RuleFor(m => m.Name)
            .TitleField(50);
        RuleFor(m => m.Price)
            .Price();
        RuleFor(m => m.SpecializationId)
            .NotEmpty();
        RuleFor(m => m.ServiceCategoryId)
            .NotEmpty();
    }
}