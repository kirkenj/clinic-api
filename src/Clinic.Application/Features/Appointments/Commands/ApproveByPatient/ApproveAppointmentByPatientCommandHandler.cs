using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByPatient;

public class
    ApproveAppointmentByPatientCommandHandler(
        IAppointmentRepository appointmentRepository,
        IOtpService otpService,
        ApproveAppointmentByPatientCommandValidator validator)
    : IRequestHandler<ApproveAppointmentByPatientCommand, Result>
{
    public async Task<Result> Handle(ApproveAppointmentByPatientCommand request,
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

        if (appointment.IsApprovedByUser)
        {
            return AppointmentErrors.AppointmentIsApproved(appointment.Id);
        }

        if (appointment.Status != AppointmentStatus.WaitingUserConfirm)
        {
            return AppointmentErrors.InvalidStatus(AppointmentStatus.WaitingUserConfirm, appointment.Status);
        }

        var isDoctorSlotTaken = await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync
            (appointment.DoctorId, appointment.DateTimeStart, appointment.DateTimeEnd, cancellationToken);
        if (isDoctorSlotTaken)
        {
            return AppointmentErrors.DateTimeDoctorTaken
                (appointment.DoctorId, appointment.DateTimeStart);
        }

        var isPatientSlotTaken = await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync
            (appointment.PatientId, appointment.DateTimeStart, appointment.DateTimeEnd, cancellationToken);
        if (isPatientSlotTaken)
        {
            return AppointmentErrors.DateTimePatientTaken
                (appointment.PatientId, appointment.DateTimeStart);
        }

        var otpKey = string.Format(ApplicationResources.ConfirmAppointmentCreationByPatientOtpKeyFormat,
            appointment.PatientId, appointment.Id);
        var otpValidationResul = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpValidationResul.IsFailure)
        {
            return otpValidationResul.Error;
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, true,
            appointment.IsApprovedByClinic, AppointmentStatus.WaitingClinicConfirm);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}