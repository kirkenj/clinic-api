using Clinic.Application.Abstractions;
using Clinic.Application.Email;
using Clinic.Application.Otp;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveCancellationByPatient;

public sealed class ApproveAppointmentCancellationByPatientCommandHandler(
    IAppointmentRepository appointmentRepository,
    IOtpService otpService,
    IEmailService emailService,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
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

        var otpKey = OtpKeys.ConfirmAppointmentCancellationByUser(request.PatientId, request.AppointmentId);
        var otpValidationResul = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpValidationResul.IsFailure)
        {
            return otpValidationResul.Error;
        }

        var patient = await patientRepository.GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.PatientId);
        }

        var appointment = await appointmentRepository.GetByIdAsync
            (request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.Confirmed);
        }

        var doctor = await doctorRepository.GetByIdAsync(appointment.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(appointment.DoctorId);
        }

        var service = await serviceRepository.GetByIdAsync(appointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return ServiceErrors.NotFound(appointment.ServiceId);
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, false,
            appointment.IsApprovedByClinic, AppointmentStatus.CancelledByUser);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        await emailService.SendAsync(doctor.Email, EmailSubjects.AppointmentCancelled,
            EmailBodies.AppointmentCancelledForDoctor(service.Name,
                patient.FirstName,
                patient.LastName, appointment.DateTimeStart),
            cancellationToken);
        return Result.Success();
    }
}