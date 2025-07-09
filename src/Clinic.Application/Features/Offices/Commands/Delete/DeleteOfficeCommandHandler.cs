using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Commands.Delete;

public sealed class DeleteOfficeCommandHandler
    (IOfficeRepository officeRepository,
    IUserAccessService userAccessService)
    : IRequestHandler<DeleteOfficeCommand, Result>
{
    public async Task<Result> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        var office = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (office is null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        await officeRepository.DeleteAsync(office, cancellationToken);
        return Result.Success();
    }
}