using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Commands.Add;

public sealed class AddSpecializationCommandHandler
    (ISpecializationRepository specializationRepository,
    IUserAccessService userAccessService,
    AddSpecializationCommandValidator validator)
    : IRequestHandler<AddSpecializationCommand, Result>
{
    public async Task<Result> Handle(AddSpecializationCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        var isExists = await specializationRepository.ExistsByNameAsync(request.Name, cancellationToken);
        if (isExists)
        {
            return SpecializationErrors.NameAlreadyTaken(request.Name);
        }

        var specialization = Specialization.Create(request.Name);
        await specializationRepository.AddAsync(specialization, cancellationToken);
        return Result.Success();
    }
}