using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.Get;

public sealed class GetOfficeQueryHandler(IOfficeRepository officeRepository)
    : IRequestHandler<GetOfficeQuery, Result<OfficeResponse>>
{
    public async Task<Result<OfficeResponse>> Handle(GetOfficeQuery request, CancellationToken cancellationToken)
    {
        var office = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (office is null)
            return Result.Failure<OfficeResponse>(
                OfficeErrors.NotFound(request.Id));

        return OfficeResponse.FromOffice(office);
    }
}