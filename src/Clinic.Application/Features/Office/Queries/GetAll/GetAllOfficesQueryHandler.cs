using Clinic.Application.Responses.Office;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.GetAll;
public sealed class GetAllOfficesQueryHandler(IOfficeRepository officeRepository) 
    : IRequestHandler<GetAllOfficesQuery, Result<IEnumerable<OfficeResponse>>>
{
    public async Task<Result<IEnumerable<OfficeResponse>>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await officeRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(OfficeResponse.FromOfficeCollection(offices));
    }
}
