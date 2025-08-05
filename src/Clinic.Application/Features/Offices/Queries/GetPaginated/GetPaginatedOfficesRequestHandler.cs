using Clinic.Application.Extensions;
using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;

public class GetPaginatedOfficesRequestHandler
    (IOfficeRepository officeRepository,
    GetPaginatedOfficesRequestValidator validator) 
    : IRequestHandler<GetPaginatedOfficesRequest,Result<PaginatedCollection<OfficeResponse>>>
{
    public async Task<Result<PaginatedCollection<OfficeResponse>>> Handle(GetPaginatedOfficesRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<OfficeResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedOffices = await officeRepository
            .GetPaginatedAsync(request.OfficeFilter, request.Page, request.PageSize, cancellationToken);
        var officeResponse = paginatedOffices.Select(OfficeResponse.FromOffice);
        return Result.Success(officeResponse);
    }
}