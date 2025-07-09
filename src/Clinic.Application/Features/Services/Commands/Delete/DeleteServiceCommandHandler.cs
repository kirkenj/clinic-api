using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Services.Commands.Delete;

public sealed class DeleteServiceCommandHandler
    (IServiceRepository serviceRepository,
    IUserAccessService userAccessService)
    : IRequestHandler<DeleteServiceCommand, Result>
{
    public async Task<Result> Handle(DeleteServiceCommand request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        var service = await serviceRepository.GetByIdAsync(request.Id, cancellationToken);
        if (service is null)
        {
            return ServiceErrors.NotFound(request.Id);
        }

        await serviceRepository.DeleteAsync(service, cancellationToken);
        return Result.Success();
    }
}