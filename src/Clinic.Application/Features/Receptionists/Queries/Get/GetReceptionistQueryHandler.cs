using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Doctors;
using Clinic.Application.Responses.Receptionists;
using Clinic.Application.Services;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.Get;

public sealed class GetReceptionistQueryHandler(IReceptionistService receptionistService)
    : IRequestHandler<GetReceptionistQuery, Result<ReceptionistResponse>>
{
    public async Task<Result<ReceptionistResponse>> Handle(GetReceptionistQuery request,
        CancellationToken cancellationToken)
    {
        return await receptionistService.GetByIdAsync(request.Id, cancellationToken);
    }
}