using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Delete;

public sealed class DeleteAppointmentResultCommandHandler
    (IAppointmentResultRepository appointmentResultRepository,
    IAppointmentRepository appointmentRepository,
    IUserAccessService userContextService)
    : IRequestHandler<DeleteAppointmentResultCommand, Result>
{
    public async Task<Result> Handle(DeleteAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        var userResult = await userContextService.ValidateUserAccessAsync(
            [RoleType.Doctor, RoleType.Receptionist], cancellationToken);
        if (userResult.IsFailure)
        {
            return userResult.Error;
        }

        if (userResult.Value.Role == RoleType.Doctor)
        {
            var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
            if (appointment is null)
            {
                return AppointmentErrors.NotFound(request.AppointmentId);
            }

            if (userResult.Value.Id != appointment.DoctorId)
            {
                return BasicErrors.NoAccess();
            }
        }

        var appointmentResult = await appointmentResultRepository.GetByAppointmentIdAsync(request.AppointmentId, cancellationToken);
        if (appointmentResult is null)
        {
            return AppointmentResultErrors.NotFoundWithAppointmentId(request.AppointmentId);
        }

        await appointmentResultRepository.DeleteAsync(appointmentResult, cancellationToken);
        return Result.Success();
    }
}