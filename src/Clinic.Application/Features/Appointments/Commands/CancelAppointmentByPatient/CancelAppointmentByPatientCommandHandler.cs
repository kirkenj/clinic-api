using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelAppointmentByPatient;

public sealed class CancelAppointmentByPatientCommandHandler(
    IAppointmentRepository appointmentRepository,
    IOtpService otpService,
    IPatientRepository patientRepository,
    INotificationService notificationService,
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

        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment == null)
        {
            return Result.Failure<string>(AppointmentErrors.NotFound(request.AppointmentId));
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
                appointment.DateTimeStart, appointment.DateTimeEnd, false,
                appointment.IsApprovedByClinic, AppointmentStatus.CancelledByUser);
            await appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return Result.Success(ApplicationResources.AppointmentSuccessfullyCancelledResponse);
        }

        var otpKey = string.Format(ApplicationResources.ConfirmAppointmentCancelByUserOtpKeyFormat,
            appointment.PatientId, appointment.Id);
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(appointment.PatientId));
        }

        var body = string.Format(ApplicationResources.ConfirmAppointmentCancelByUserMessageBody,
            otpGenerationResult.Value);
        await notificationService.NotifyAsync(patient,
            ApplicationResources.ConfirmAppointmentCancelByUserMessageSubject,
            body, cancellationToken);

        return Result.Success(ApplicationResources.CheckEmailForFurtherDetailsResponse);
    }
}