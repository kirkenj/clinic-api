using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.Get;

public sealed class GetReceptionistQueryHandler(IReceptionistRepository receptionistRepository)
    : IRequestHandler<GetReceptionistQuery, Result<ReceptionistResponse>>
{
    public async Task<Result<ReceptionistResponse>> Handle(GetReceptionistQuery request, CancellationToken cancellationToken)
    {
        var receptionist = await receptionistRepository.GetByIdAsync(request.Id, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<ReceptionistResponse>(
                ReceptionistErrors.NotFound(request.Id));
        }

        return ReceptionistResponse.FromReceptionist(receptionist);
    }
}