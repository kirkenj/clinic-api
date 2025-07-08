using Clinic.Application.Extensions;
using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;

public class GetPaginatedAppointmentResultsRequestHandler(
    ILogger<GetPaginatedAppointmentResultsRequestHandler> logger,
    IAppointmentResultRepository appointmentResultRepository,
    GetPaginatedAppointmentResultsRequestValidator validator)
    : IRequestHandler<GetPaginatedAppointmentResultsRequest, Result<PaginatedCollection<AppointmentResultResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResultResponse>>> Handle(
        GetPaginatedAppointmentResultsRequest request, CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedAppointmentResultsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<AppointmentResultResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedAppointmentResults = await appointmentResultRepository.GetPaginatedAsync(
            request.AppointmentResultFilter, request.Page, request.PageSize, cancellationToken);
        var appointmentResultResponse = paginatedAppointmentResults
            .Select(AppointmentResultResponse.FromAppointmentResult);
        logger.LogInformation("PaginatedAppointmentResultsResponse: {@response}", appointmentResultResponse);
        return Result.Success(appointmentResultResponse);
    }
}