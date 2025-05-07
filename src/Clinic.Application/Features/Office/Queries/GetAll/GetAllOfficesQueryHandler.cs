using Clinic.Application.Dtos.Office;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.GetAll;
public sealed class GetAllOfficesQueryHandler(IOfficeRepository officeRepository) : IRequestHandler<GetAllOfficesQuery, Result<IEnumerable<OfficeDto>>>
{
    public async Task<Result<IEnumerable<OfficeDto>>> Handle(GetAllOfficesQuery request, CancellationToken cancellationToken)
    {
        var offices = await officeRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(offices.Select(OfficeDto.FromOffice));
    }
}
