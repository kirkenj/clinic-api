using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public class
    ApproveAppointmentCancellationByPatientCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOtpService otpService,
        ApproveAppointmentCancellationByPatientCommandValidator validator)
    : IRequestHandler<ApproveAppointmentCancellationByPatientCommand, Result>
{
    public async Task<Result> Handle(ApproveAppointmentCancellationByPatientCommand request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointment = await appointmentRepository.GetByIdAsync
            (request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return AppointmentErrors.InvalidStatus(AppointmentStatus.Confirmed, appointment.Status);
        }

        var otpKey = string.Format(ApplicationResources.ConfirmAppointmentCancelByUserOtpKeyFormat,
            appointment.PatientId, appointment.Id);
        var otpValidationResul = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpValidationResul.IsFailure)
        {
            return otpValidationResul.Error;
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, false,
            appointment.IsApprovedByClinic, AppointmentStatus.CancelledByUser);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}