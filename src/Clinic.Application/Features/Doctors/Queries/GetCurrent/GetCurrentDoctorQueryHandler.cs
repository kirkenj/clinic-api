using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetCurrent;

public sealed class GetCurrentDoctorQueryHandler
    (IDoctorService doctorService,
    IUserAccessService userAccessService)
    : IRequestHandler<GetCurrentDoctorQuery, Result<DoctorResponse>>
{
    public async Task<Result<DoctorResponse>> Handle(GetCurrentDoctorQuery request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Doctor], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<DoctorResponse>(userAccessResult.Error);
        }

        return await doctorService.GetByIdAsync(userAccessResult.Value.Id, cancellationToken);
    }
}