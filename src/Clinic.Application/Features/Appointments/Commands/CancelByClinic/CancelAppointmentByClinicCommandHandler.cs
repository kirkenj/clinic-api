using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByClinic;

public sealed class CancelAppointmentByClinicCommandHandler(
    IAppointmentService appointmentService,
    IUserAccessService userAccessService,
    CancelAppointmentByClinicCommandValidator validator)
    : IRequestHandler<CancelAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentByClinicCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist, RoleType.Doctor], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        if (userAccessResult.Value.Role == RoleType.Doctor)
        {
            var appointmentResult = await appointmentService.GetByIdAsync(
            request.AppointmentId, cancellationToken);
            if (appointmentResult.IsFailure)
            {
                return appointmentResult.Error;
            }

            if (appointmentResult.Value.DoctorId != userAccessResult.Value.Id)
            {
                return BasicErrors.NoAccess();
            }
        }

        return await appointmentService.CancelByClinicAsync(request.AppointmentId, cancellationToken);
    }
}