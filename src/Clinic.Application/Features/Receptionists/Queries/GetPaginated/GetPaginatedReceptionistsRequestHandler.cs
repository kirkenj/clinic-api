using Clinic.Application.Extensions;
using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Receptionists.Queries.GetPaginated;

public class GetPaginatedReceptionistsRequestHandler(ILogger<GetPaginatedReceptionistsRequestHandler> logger,
    IReceptionistRepository receptionistRepository,
    GetPaginatedReceptionistsRequestValidator validator) : IRequestHandler<GetPaginatedReceptionistsRequest,
    Result<PaginatedCollection<ReceptionistResponse>>>
{
    public async Task<Result<PaginatedCollection<ReceptionistResponse>>> Handle(
        GetPaginatedReceptionistsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedReceptionistsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<ReceptionistResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedReceptionists =
            await receptionistRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var receptionistResponse = paginatedReceptionists.Select(ReceptionistResponse.FromReceptionist);
        logger.LogInformation("PaginatedReceptionistsResponse: {@response}", receptionistResponse);
        return Result.Success(receptionistResponse);
    }
}