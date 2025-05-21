using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.ServiceCategories.Commands.Delete;

public sealed class DeleteServiceCategoryCommandHandler(IServiceCategoryRepository serviceCategoryRepository)
    : IRequestHandler<DeleteServiceCategoryCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceCategoryCommand request, CancellationToken cancellationToken)
    {
        var serviceCategory = await serviceCategoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (serviceCategory is null)
        {
            return ServiceCategoryErrors.NotFound(request.Id);
        }

        await serviceCategoryRepository.DeleteAsync(serviceCategory, cancellationToken);
        return Result.Success();
    }
}