using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetAll;
public sealed class GetAllReceptionistsQueryHandler(IReceptionistRepository receptionistRepository)
    : IRequestHandler<GetAllReceptionistsQuery, Result<IEnumerable<ReceptionistResponse>>>
{
    public async Task<Result<IEnumerable<ReceptionistResponse>>> Handle(GetAllReceptionistsQuery request, CancellationToken cancellationToken)
    {
        var receptionists = await receptionistRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(ReceptionistResponse.FromReceptionistCollection(receptionists));
    }
}
