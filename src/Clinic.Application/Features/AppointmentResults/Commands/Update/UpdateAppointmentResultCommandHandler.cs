using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Update;

public sealed class UpdateAppointmentResultCommandHandler(
    IAppointmentResultRepository appointmentResultRepository,
    IAppointmentRepository appointmentRepository,
    IUserAccessService userAccessService,
    UpdateAppointmentResultCommandValidator validator)
    : IRequestHandler<UpdateAppointmentResultCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentResultCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userResult = await userAccessService.ValidateUserAccessAsync(
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

        appointmentResult.Update(request.Complaints, request.Conclusion,
            request.Recommendations, request.AppointmentId);
        await appointmentResultRepository.UpdateAsync(appointmentResult, cancellationToken);
        return Result.Success();
    }
}