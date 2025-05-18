using Clinic.Application.Extensions;
using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Specializations.Queries.GetPaginated;

public class GetPaginatedSpecializationsRequestHandler(ILogger<GetPaginatedSpecializationsRequestHandler> logger,
    ISpecializationRepository specializationRepository,
    GetPaginatedSpecializationsRequestValidator validator) : IRequestHandler<GetPaginatedSpecializationsRequest,
    Result<PaginatedCollection<SpecializationResponse>>>
{
    public async Task<Result<PaginatedCollection<SpecializationResponse>>> Handle(
        GetPaginatedSpecializationsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedSpecializationsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<SpecializationResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedSpecializations =
            await specializationRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var specializationResponse = paginatedSpecializations.Select(SpecializationResponse.FromSpecialization);
        logger.LogInformation("PaginatedSpecializationsResponse: {@response}", specializationResponse);
        return Result.Success(specializationResponse);
    }
}