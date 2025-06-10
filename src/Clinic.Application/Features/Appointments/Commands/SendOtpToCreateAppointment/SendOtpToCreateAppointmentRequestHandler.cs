using Clinic.Application.Abstractions;
using Clinic.Application.Emails;
using Clinic.Application.Otp;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SendOtpToCreateAppointment;

public sealed class SendOtpToCreateAppointmentRequestHandler(
    SendOtpToCreateAppointmentRequestValidator validator,
    IAppointmentRepository appointmentRepository,
    IOtpService otpService,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    IPatientRepository patientRepository,
    IEmailService emailService) : IRequestHandler<SendOtpToCreateAppointmentRequest, Result>
{
    public async Task<Result> Handle(SendOtpToCreateAppointmentRequest request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.Status != AppointmentStatus.WaitingUserConfirm)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.WaitingUserConfirm);
        }

        if (appointment.DateTimeStart <= DateTime.UtcNow)
        {
            return AppointmentErrors.InvalidDate(appointment.DateTimeStart);
        }

        var otpKey = OtpKeys.ConfirmAppointmentCreationByPatient(appointment.PatientId, appointment.Id);
        var otpResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpResult.IsFailure)
        {
            return otpResult.Error;
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

        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(appointment.PatientId);
        }

        await emailService.SendAsync(patient.Email, EmailSubjects.ConfirmAppointment,
            EmailBodies.ConfirmAppointment(service.Name, doctor.FirstName, doctor.LastName,
                service.Price, appointment.DateTimeStart, otpResult.Value),
            cancellationToken);
        return Result.Success();
    }
}