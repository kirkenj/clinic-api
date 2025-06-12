using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginatedForDoctor;

public class GetPaginatedAppointmentsForDoctorRequestHandler(
    ILogger<GetPaginatedAppointmentsForDoctorRequestHandler> logger,
    IAppointmentRepository appointmentRepository,
    GetPaginatedAppointmentsForDoctorRequestValidator validator)
    : IRequestHandler<GetPaginatedAppointmentsForDoctorRequest,
        Result<PaginatedCollection<AppointmentResponse>>>
{
    public async Task<Result<PaginatedCollection<AppointmentResponse>>> Handle(
        GetPaginatedAppointmentsForDoctorRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedAppointmentsForDoctorRequestHandler: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<AppointmentResponse>>
                (Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedAppointments = await appointmentRepository
            .GetDoctorsAppointments(request.DoctorId, request.Page, request.PageSize, cancellationToken);
        var appointmentResponse = paginatedAppointments
            .Select(AppointmentResponse.FromAppointment);
        logger.LogInformation("PaginatedAppointmentsResponse: {@response}", appointmentResponse);
        return Result.Success(appointmentResponse);
    }
}