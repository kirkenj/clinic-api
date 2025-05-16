using Clinic.Application.Extensions;
using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;

public class GetPaginatedOfficesRequestHandler(ILogger<GetPaginatedOfficesRequestHandler> logger,
    IOfficeRepository officeRepository,
    GetPaginatedOfficesRequestValidator validator) : IRequestHandler<GetPaginatedOfficesRequest,
    Result<PaginatedCollection<OfficeResponse>>>
{
    public async Task<Result<PaginatedCollection<OfficeResponse>>> Handle(GetPaginatedOfficesRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedOfficesRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<OfficeResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedOffices =
            await officeRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var officeResponse = paginatedOffices.Select(OfficeResponse.FromOffice);
        logger.LogInformation("PaginatedOfficesResponse: {@response}", officeResponse);
        return Result.Success(officeResponse);
    }
}