using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetCurrent;

public sealed class GetCurrentReceptionistQueryHandler
    (IReceptionistService receptionistService,
    IUserAccessService userAccessService)
    : IRequestHandler<GetCurrentReceptionistQuery, Result<ReceptionistResponse>>
{
    public async Task<Result<ReceptionistResponse>> Handle(GetCurrentReceptionistQuery request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<ReceptionistResponse>(userAccessResult.Error);
        }

        return await receptionistService.GetByIdAsync(userAccessResult.Value.Id, cancellationToken);
    }
}