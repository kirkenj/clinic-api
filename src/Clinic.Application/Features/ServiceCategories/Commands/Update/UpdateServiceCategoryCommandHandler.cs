using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Update;

public sealed class UpdateServiceCategoryCommandHandler
    (IServiceCategoryRepository serviceCategoryRepository,
        UpdateServiceCategoryCommandValidator validator)
    : IRequestHandler<UpdateServiceCategoryCommand, Result>
{
    public async Task<Result> Handle(UpdateServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var serviceCategory = await serviceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (serviceCategory is null)
        {
            return ServiceCategoryErrors.NotFound(request.Id);
        }

        if (!serviceCategory.IsActive)
        {
            return ServiceCategoryErrors.NotActive(serviceCategory.Id);
        }

        var hasSameName = serviceCategory.Name.HasSameValue(request.Name);
        if (!hasSameName)
        {
            var isNameTaken =
                await serviceCategoryRepository.ExistsByNameAsync(request.Name, cancellationToken);
            if (isNameTaken)
            {
                return ServiceCategoryErrors.NameAlreadyTaken(request.Name);
            }
        }

        serviceCategory.Update(request.Name, request.Duration);
        await serviceCategoryRepository.UpdateAsync(serviceCategory, cancellationToken);
        return Result.Success();
    }
}