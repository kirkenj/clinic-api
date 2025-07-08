using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByPatient;

public sealed class CancelAppointmentByPatientCommandHandler(
    IUserAccessService userAccessService,
    IAppointmentService appointmentService,
    CancelAppointmentByPatientCommandValidator validator)
    : IRequestHandler<CancelAppointmentByPatientCommand, Result<string>>
{
    public async Task<Result<string>> Handle(CancelAppointmentByPatientCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Receptionist, RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<string>(userAccessResult.Error);
        }

        if (userAccessResult.Value.Role == RoleType.Patient) 
        {
            var appointmentResult = await appointmentService.GetByIdAsync(
                request.AppointmentId, cancellationToken);
            if (appointmentResult.IsFailure)
            {
                return Result.Failure<string>(appointmentResult.Error);
            }

            if (appointmentResult.Value.PatientId != userAccessResult.Value.Id)
            {
                return Result.Failure<string>(BasicErrors.NoAccess());
            }
        }

        return await appointmentService.CancelByPatientAsync(request.AppointmentId, cancellationToken);
    }
}