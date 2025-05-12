using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Add;
public sealed class AddSpecializationCommandHandler(
    ISpecializationRepository specializationRepository,
    AddSpecializationCommandValidator validator
    ) : IRequestHandler<AddSpecializationCommand, Result>
{
    public async Task<Result> Handle(AddSpecializationCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var trimmedName = request.Name.Trim();
        var isExists = await specializationRepository.ExistsByNameAsync(trimmedName, cancellationToken);
        if (isExists)
        {
            return SpecializationErrors.NameAlreadyTaken(trimmedName);
        }

        var specialization = Specialization.Create(trimmedName);

        await specializationRepository.AddAsync(specialization, cancellationToken);

        return Result.Success();
    }
}