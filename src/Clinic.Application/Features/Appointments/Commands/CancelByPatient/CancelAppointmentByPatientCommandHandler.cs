using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByPatient;

public sealed class CancelAppointmentByPatientCommandHandler(
    IAppointmentRepository appointmentRepository,
    IOtpService otpService,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    IEmailService emailService,
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

        if (appointment.Status is AppointmentStatus.WaitingUserConfirm or AppointmentStatus.WaitingClinicConfirm)
        {
            appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
                appointment.DateTimeStart, appointment.DateTimeEnd, false,
                appointment.IsApprovedByClinic, AppointmentStatus.CancelledByUser);
            await appointmentRepository.UpdateAsync(appointment, cancellationToken);
            return Result.Success("Appointment cancelled. No further actions needed");
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return Result.Failure<string>(AppointmentErrors.InvalidStatus(appointment.Status,
                AppointmentStatus.WaitingUserConfirm, AppointmentStatus.WaitingClinicConfirm,
                AppointmentStatus.Confirmed));
        }

        var otpKey = $"ConfirmAppointmentCancellationByPatient{appointment.PatientId}:{appointment.Id}";
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

        var doctor = await doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(appointment.DoctorId));
        }

        var service = await serviceRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(appointment.ServiceId));
        }

        await emailService.SendAsync(patient.Email,
            Constants.SubjectConfirmAppointmentCancellationByUser,
            EmailBodyBuilder.BuildConfirmAppointmentCancellationContent(service.Name, doctor.FirstName, doctor.LastName,
                appointment.DateTimeStart, otpGenerationResult.Value), cancellationToken);
        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }
}