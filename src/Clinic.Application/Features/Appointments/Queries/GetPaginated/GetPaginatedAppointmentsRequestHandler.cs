using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated;

public class GetPaginatedAppointmentsRequestHandler(
    ILogger<GetPaginatedAppointmentsRequestHandler> logger,
    IAppointmentRepository appointmentRepository,
    GetPaginatedAppointmentsRequestValidator validator) : IRequestHandler<GetPaginatedAppointmentsRequest,
    Result<PaginatedCollection<AppointmentResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResponse>>> Handle(GetPaginatedAppointmentsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedAppointmentsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<AppointmentResponse>>
                (Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedAppointments =
            await appointmentRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var appointmentResponse = paginatedAppointments
            .Select(AppointmentResponse.FromAppointment);
        logger.LogInformation("PaginatedAppointmentsResponse: {@response}", appointmentResponse);
        return Result.Success(appointmentResponse);
    }
}