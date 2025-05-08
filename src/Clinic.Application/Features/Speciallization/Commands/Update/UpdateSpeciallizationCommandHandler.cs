using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Update;
public sealed class UpdateSpeciallizationCommandHandler(
    ISpeciallizationRepository speciallizationRepository,
    UpdateSpeciallizationCommandValidator validator
    ) : IRequestHandler<UpdateSpeciallizationCommand, Result>
{
    public async Task<Result> Handle(UpdateSpeciallizationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var speciallization = await speciallizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciallization == null)
        {
            return SpeciallizationErrors.NotFound(request.Id);
        }

        if (speciallization.Name != request.Name)
        {
            if (await speciallizationRepository.ExistsByNameAsync(request.Name, cancellationToken))
            {
                return SpeciallizationErrors.NameAlreadyTaken(request.Name);
            }
        }

        speciallization.Update(request.Name);

        await speciallizationRepository.UpdateAsync(speciallization, cancellationToken);

        return Result.Success();
    }
}