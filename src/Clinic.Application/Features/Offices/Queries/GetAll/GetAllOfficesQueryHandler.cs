using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetAll;

public sealed class GetAllOfficesQueryHandler(IOfficeRepository officeRepository)
    : IRequestHandler<GetAllOfficesQuery, Result<IEnumerable<OfficeResponse>>>
{
    public async Task<Result<IEnumerable<OfficeResponse>>> Handle(GetAllOfficesQuery request,
        CancellationToken cancellationToken)
    {
        var offices = await officeRepository.GetAllAsync(cancellationToken);
        return Result.Success(OfficeResponse.FromOfficeCollection(offices));
    }
}