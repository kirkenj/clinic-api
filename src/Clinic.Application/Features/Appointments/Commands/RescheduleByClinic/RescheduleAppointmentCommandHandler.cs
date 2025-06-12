using Clinic.Application.Abstractions;
using Clinic.Application.Email;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.RescheduleByClinic;

public sealed class RescheduleAppointmentCommandHandler(
    IDoctorRepository doctorRepository,
    IAppointmentRepository appointmentRepository,
    IServiceRepository serviceRepository,
    ISlotService slotService,
    IPatientRepository patientRepository,
    IEmailService emailService,
    RescheduleAppointmentCommandValidator validator)
    : IRequestHandler<RescheduleAppointmentCommand, Result>
{
    public async Task<Result> Handle(RescheduleAppointmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var slot = slotService.GetSlot(request.NewDateTime.ToUniversalTime());
        if (slot == null)
        {
            return AppointmentErrors.NoSlotsFound(request.NewDateTime);
        }

        if (slot.SlotStart <= DateTime.UtcNow)
        {
            return AppointmentErrors.InvalidDate(request.NewDateTime);
        }

        var oldAppointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (oldAppointment == null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (!oldAppointment.IsApprovedByUser)
        {
            return AppointmentErrors.AppointmentIsNotApprovedByUser(oldAppointment.Id);
        }

        if (oldAppointment.Status is not (AppointmentStatus.Confirmed or AppointmentStatus.WaitingClinicConfirm))
        {
            return AppointmentErrors.InvalidStatus(oldAppointment.Status, AppointmentStatus.Confirmed,
                AppointmentStatus.WaitingClinicConfirm);
        }

        var oldDoctor = await doctorRepository
            .GetByIdAsync(oldAppointment.DoctorId, cancellationToken);

        if (oldDoctor is null)
        {
            return DoctorErrors.NotFound(oldAppointment.DoctorId);
        }

        var newDoctor = oldAppointment.DoctorId != request.NewDoctorId
            ? await doctorRepository.GetByIdAsync(request.NewDoctorId, cancellationToken)
            : oldDoctor;

        if (newDoctor is null)
        {
            return DoctorErrors.NotFound(request.NewDoctorId);
        }

        if (!newDoctor.IsActive)
        {
            return DoctorErrors.NotActive(request.NewDoctorId);
        }

        var patient = await patientRepository
            .GetByIdAsync(oldAppointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(oldAppointment.DoctorId);
        }

        if (!patient.IsActive)
        {
            return PatientErrors.NotActive(oldAppointment.DoctorId);
        }

        var service = await serviceRepository
            .GetByIdAsync(oldAppointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(oldAppointment.ServiceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<string>(ServiceErrors.NotActive(oldAppointment.ServiceId));
        }

        var isDoctorSlotTaken = await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync
            (request.NewDoctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isDoctorSlotTaken)
        {
            return AppointmentErrors.DateTimeDoctorTaken(request.NewDoctorId, request.NewDateTime);
        }

        var isPatientSlotTaken = await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync
            (oldAppointment.PatientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isPatientSlotTaken)
        {
            return AppointmentErrors.DateTimePatientTaken(oldAppointment.PatientId, request.NewDateTime);
        }

        oldAppointment.Update(oldAppointment.DoctorId, oldAppointment.PatientId, oldAppointment.ServiceId,
            oldAppointment.DateTimeStart, oldAppointment.DateTimeEnd, oldAppointment.IsApprovedByUser,
            oldAppointment.IsApprovedByClinic, AppointmentStatus.Rescheduled);
        await appointmentRepository.UpdateAsync(oldAppointment, cancellationToken);
        var newAppointment = Appointment.Create(request.NewDoctorId, oldAppointment.PatientId, oldAppointment.ServiceId,
            slot.SlotStart, slot.SlotEnd, true, true, AppointmentStatus.Confirmed);
        await appointmentRepository.AddAsync(newAppointment, cancellationToken);

        await Task.WhenAll(
            emailService.SendAsync(oldDoctor.Email, EmailSubjects.AppointmentCancelled,
                EmailBodies.AppointmentCancelledForDoctor(service.Name,
                    patient.FirstName, patient.LastName, oldAppointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(newDoctor.Email, EmailSubjects.NewAppointment,
                EmailBodies.NewAppointmentForDoctor(service.Name,
                    patient.FirstName, patient.LastName, newAppointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(patient.Email, EmailSubjects.AppointmentRescheduled,
                EmailBodies.AppointmentRescheduled(service.Name,
                    oldDoctor.FirstName, oldDoctor.LastName, oldAppointment.DateTimeStart,
                    newDoctor.FirstName, newDoctor.LastName, newAppointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }
}