using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Update;
public sealed class UpdateSpecializationCommandHandler(
    ISpecializationRepository specializationRepository,
    UpdateSpecializationCommandValidator validator
    ) : IRequestHandler<UpdateSpecializationCommand, Result>
{
    public async Task<Result> Handle(UpdateSpecializationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return SpecializationErrors.NotFound(request.Id);
        }

        if (!specialization.IsActive)
        {
            return SpecializationErrors.NotActive(specialization.Id);
        }

        var trimmedName = request.Name.Trim();
        if (specialization.Name != trimmedName)
        {
            var isExists = await specializationRepository.ExistsByNameAsync(trimmedName, cancellationToken);
            if (isExists)
            {
                return SpecializationErrors.NameAlreadyTaken(trimmedName);
            }
        }

        specialization.Update(trimmedName);

        await specializationRepository.UpdateAsync(specialization, cancellationToken);

        return Result.Success();
    }
}