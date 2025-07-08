using Clinic.Application.Abstractions;
using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetByAppointmentId;

public sealed class GetAppointmentResultByAppointmentIdQueryHandler
    (IAppointmentResultRepository appointmentResultRepository,
    IAppointmentRepository appointmentRepository,
    IUserAccessService userAccessService)
    : IRequestHandler<GetAppointmentResultByAppointmentIdQuery, Result<AppointmentResultResponse>>
{
    public async Task<Result<AppointmentResultResponse>> Handle
        (GetAppointmentResultByAppointmentIdQuery request, CancellationToken cancellationToken)
    {
        var userResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Doctor, RoleType.Receptionist, RoleType.Patient], cancellationToken);
        if (userResult.IsFailure)
        {
            return Result.Failure<AppointmentResultResponse>(userResult.Error);
        }

        if (userResult.Value.Role == RoleType.Patient)
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment is null)
            {
                return Result.Failure<AppointmentResultResponse>(
                    AppointmentErrors.NotFound(request.AppointmentId));
            }

            if (userResult.Value.Id != appointment.PatientId)
            {
                return Result.Failure<AppointmentResultResponse>(
                    BasicErrors.NoAccess());
            }
        }


        var appointmentResult = await appointmentResultRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (appointmentResult is null)
        {
            return Result.Failure<AppointmentResultResponse>(
                AppointmentResultErrors.NotFoundWithAppointmentId(request.AppointmentId));
        }

        return AppointmentResultResponse.FromAppointmentResult(appointmentResult);
    }
}