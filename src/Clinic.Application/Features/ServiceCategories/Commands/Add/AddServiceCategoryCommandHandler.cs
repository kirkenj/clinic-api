using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Add;

public sealed class AddServiceCategoryCommandHandler
    (IServiceCategoryRepository serviceCategoryRepository,
        AddServiceCategoryCommandValidator validator)
    : IRequestHandler<AddServiceCategoryCommand, Result>
{
    public async Task<Result> Handle(AddServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var isNameTaken =
            await serviceCategoryRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (isNameTaken)
        {
            return ServiceCategoryErrors.NameAlreadyTaken(request.Name);
        }

        var serviceCategory = ServiceCategory.Create(request.Name, request.Duration);
        await serviceCategoryRepository.AddAsync(serviceCategory, cancellationToken);
        return Result.Success();
    }
}