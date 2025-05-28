using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Features.Services.Commands.Add;

public sealed class AddServiceCommandHandler
    (IServiceRepository serviceRepository,
    ISpecializationRepository specializationRepository,
    AddServiceCommandValidator validator,
    IOptions<ServiceOptions> serviceOptions)
    : IRequestHandler<AddServiceCommand, Result>
{
    public async Task<Result> Handle(AddServiceCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (request.Price > serviceOptions.Value.MaximalCost || 
            request.Price < serviceOptions.Value.MinimalCost)
        {
            return ServiceErrors.PriceOutOfRange(serviceOptions.Value.MinimalCost, 
                serviceOptions.Value.MaximalCost);
        }

        var isNameTaken = await serviceRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (isNameTaken)
        {
            return ServiceErrors.NameAlreadyTaken(request.Name);
        }

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

        var service = Service.Create(request.Name, request.Price, request.SpecializationId);
        await serviceRepository.AddAsync(service, cancellationToken);
        return Result.Success();
    }
}