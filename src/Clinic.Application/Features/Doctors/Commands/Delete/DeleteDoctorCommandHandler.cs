using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Delete;

public sealed class DeleteDoctorCommandHandler
    (IDoctorService doctorService,
    IUserAccessService userAccessService)
    : IRequestHandler<DeleteDoctorCommand, Result>
{
    public async Task<Result> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        return await doctorService.DeleteAsync(request.Id, cancellationToken);
    }
}