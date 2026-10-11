using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;

public sealed class SendOtpToCreateAppointmentRequestHandler(
    SendOtpToCreateAppointmentRequestValidator validator,
    IAppointmentService appointmentService,
    IUserAccessService userAccessService)
    : IRequestHandler<SendOtpToCreateAppointmentRequest, Result>
{
    public async Task<Result> Handle(SendOtpToCreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.AllRoles, cancellationToken);
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

        return await appointmentService.SendOtpToCreateAppointment(
            request.AppointmentId, cancellationToken);
    }
}