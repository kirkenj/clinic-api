using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;
public class GetPaginatedOfficesRequestHandler(
    ILogger<GetPaginatedOfficesRequestHandler> logger,
    IOfficeRepository officeRepository,
    GetPaginatedOfficesRequestValidator validator
    )
    : IRequestHandler<GetPaginatedOfficesRequest, Result<IEnumerable<OfficeResponse>>>
{
    public async Task<Result<IEnumerable<OfficeResponse>>> Handle(GetPaginatedOfficesRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Got request: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<IEnumerable<OfficeResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var offices = await officeRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var officeResponses = offices.Select(OfficeResponse.FromOffice);
        logger.LogInformation("Sending response: {@response}", officeResponses);

        return Result.Success(officeResponses);
    }
}