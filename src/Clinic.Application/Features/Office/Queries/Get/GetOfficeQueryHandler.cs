using Clinic.Application.Dtos.Office;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Queries.Get;

public sealed class GetOfficeQueryHandler(IOfficeRepository officeRepository) : IRequestHandler<GetOfficeQuery, Result<OfficeDto>>
{
    public async Task<Result<OfficeDto>> Handle(GetOfficeQuery request, CancellationToken cancellationToken)
    {
        var office = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (office == null)
        {
            return Result.Failure<OfficeDto>(
                OfficeErrors.NotFound(request.Id));
        }

        return OfficeDto.FromOffice(office);
    }
}