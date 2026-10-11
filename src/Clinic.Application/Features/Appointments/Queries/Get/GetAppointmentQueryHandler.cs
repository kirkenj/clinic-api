using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Appointments;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.Get.Staff;

public sealed class GetAppointmentQueryHandler
    (IAppointmentService appointmentService,
    IUserAccessService userAccessService)
    : IRequestHandler<GetAppointmentQuery, Result<AppointmentResponse>>
{
    public async Task<Result<AppointmentResponse>> Handle(GetAppointmentQuery request,
        CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.AllRoles, cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<AppointmentResponse>(userAccessResult.Error);
        }

        var appointmentResult = await appointmentService.GetByIdAsync(request.Id, cancellationToken);
        if (appointmentResult.IsFailure)
        {
            return Result.Failure<AppointmentResponse>(appointmentResult.Error);
        }

        if (userAccessResult.Value.Role == RoleType.Patient &&
            userAccessResult.Value.Id != appointmentResult.Value.PatientId)
        {
            return Result.Failure<AppointmentResponse>(BasicErrors.NoAccess());
        }

        return appointmentResult;
    }
}