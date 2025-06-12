using Clinic.Application.Abstractions;
using Clinic.Application.Emails;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.ApproveByClinic;

public sealed class ApproveAppointmentByClinicCommandHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    IEmailService emailService,
    ApproveAppointmentByClinicCommandValidator validator)
    : IRequestHandler<ApproveAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(ApproveAppointmentByClinicCommand request, CancellationToken cancellationToken)
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

        if (appointment.IsApprovedByClinic)
        {
            return AppointmentErrors.AppointmentIsApproved(appointment.Id);
        }

        if (!appointment.IsApprovedByUser)
        {
            return AppointmentErrors.AppointmentIsNotApprovedByUser(appointment.PatientId);
        }

        var service = await serviceRepository
            .GetByIdAsync(appointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(appointment.ServiceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<string>(ServiceErrors.NotActive(appointment.ServiceId));
        }

        var doctor = await doctorRepository
            .GetByIdAsync(appointment.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(appointment.DoctorId));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(appointment.DoctorId));
        }

        var isDoctorSlotTaken = await appointmentRepository
            .ExistsApprovedWithDoctorAndDatetimeAsync(appointment.DoctorId,
                appointment.DateTimeStart,
                appointment.DateTimeEnd,
                cancellationToken);
        if (isDoctorSlotTaken)
        {
            return AppointmentErrors.DateTimeDoctorTaken(appointment.DoctorId, appointment.DateTimeStart);
        }

        var patient = await patientRepository
            .GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(appointment.DoctorId));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(appointment.DoctorId));
        }

        var isPatientSlotTaken = await appointmentRepository
            .ExistsApprovedWithPatientAndDatetimeAsync(appointment.PatientId,
                appointment.DateTimeStart,
                appointment.DateTimeEnd,
                cancellationToken);
        if (isPatientSlotTaken)
        {
            return AppointmentErrors.DateTimePatientTaken(appointment.PatientId, appointment.DateTimeStart);
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, appointment.IsApprovedByUser,
            true, AppointmentStatus.Confirmed);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        await Task.WhenAll(
            emailService.SendAsync(patient.Email, "Appointment approved",
                $"Your appointment for '{service.Name}'({service.Price}) at {appointment.DateTimeStart} has been approved",
                cancellationToken),
            emailService.SendAsync(doctor.Email,
                EmailSubjects.NewAppointment,
                EmailBodies.NewAppointmentForDoctor(service.Name, patient.FirstName, patient.LastName,
                    appointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }
}