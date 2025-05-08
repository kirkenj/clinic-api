using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Commands.Add;
public sealed class AddSpeciallizationCommandHandler(
    ISpeciallizationRepository speciallizationRepository,
    AddSpeciallizationCommandValidator validator
    ) : IRequestHandler<AddSpeciallizationCommand, Result>
{
    public async Task<Result> Handle(AddSpeciallizationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        if (await speciallizationRepository.ExistsByNameAsync(request.Name, cancellationToken))
        {
            return SpeciallizationErrors.NameAlreadyTaken(request.Name);
        }

        var speciallization = Domain.Speciallization.Speciallization.Create(request.Name);

        await speciallizationRepository.AddAsync(speciallization, cancellationToken);

        return Result.Success();
    }
}