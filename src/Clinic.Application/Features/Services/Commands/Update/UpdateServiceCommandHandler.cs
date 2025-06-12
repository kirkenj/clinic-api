using Clinic.Application.Extensions;
using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Features.Services.Commands.Update;

public sealed class UpdateServiceCommandHandler
    (IServiceRepository serviceRepository,
    ISpecializationRepository specializationRepository,
    UpdateServiceCommandValidator validator,
    IOptions<ServiceOptions> serviceOptions)
    : IRequestHandler<UpdateServiceCommand, Result>
{
    public async Task<Result> Handle(UpdateServiceCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var service = await serviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (service is null)
        {
            return ServiceErrors.NotFound(request.Id);
        }

        if (!service.IsActive)
        {
            return ServiceErrors.NotActive(service.Id);
        }

        var hasSameName = service.Name.HasSameValue(request.Name);
        if (!hasSameName)
        {
            var isNameTaken = await serviceRepository.ExistsByNameAsync(request.Name, cancellationToken);
            if (isNameTaken)
            {
                return ServiceErrors.NameAlreadyTaken(request.Name);
            }
        }

        if (request.Price != service.Price)
        {
            if (request.Price > serviceOptions.Value.MaximalCost || 
                request.Price < serviceOptions.Value.MinimalCost)
            {
                return ServiceErrors.PriceOutOfRange(serviceOptions.Value.MinimalCost,
                    serviceOptions.Value.MaximalCost);
            }
        }

        if (request.SpecializationId != service.SpecializationId)
        {
            var specialization =
                await specializationRepository.GetByIdAsync(request.SpecializationId, cancellationToken);
            if (specialization is null)
            {
                return SpecializationErrors.NotFound(request.SpecializationId);
            }

            if (!specialization.IsActive)
            {
                return SpecializationErrors.NotActive(request.SpecializationId);
            }
        }

        service.Update(request.Name, request.Price,
            request.SpecializationId, request.ServiceCategoryId);
        await serviceRepository.UpdateAsync(service, cancellationToken);
        return Result.Success();
    }
}