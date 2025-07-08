using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public sealed class ApproveAppointmentCancellationByPatientCommandHandler(
    IUserAccessService userAccessService,
    IAppointmentService appointmentService,
    ApproveAppointmentCancellationByPatientCommandValidator validator)
    : IRequestHandler<ApproveAppointmentCancellationByPatientCommand, Result>
{
    public async Task<Result> Handle(ApproveAppointmentCancellationByPatientCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Patient, RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return userAccessResult.Error;
        }

        if (userAccessResult.Value.Role == RoleType.Patient) 
        {
            var appointmentResult = await appointmentService.GetByIdAsync(
                request.AppointmentId, cancellationToken);
            if (appointmentResult.IsFailure)
            {
                return appointmentResult.Error;
            }

            if (appointmentResult.Value.PatientId != userAccessResult.Value.Id)
            {
                return BasicErrors.NoAccess();
            }
        }

        return await appointmentService.ApproveAppointmentCancellationByPatient(
            request.AppointmentId, request.Otp, cancellationToken);
    }
}