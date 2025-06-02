using Clinic.Application.Services.SlotService;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandHandler(
    IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    UpdateAppointmentCommandValidator validator,
    ISlotService slotService)
    : IRequestHandler<UpdateAppointmentCommand, Result>
{
    public async Task<Result> Handle(UpdateAppointmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var slot = slotService.GetSlot(request.DateTime);
        if (slot == null)
        {
            return AppointmentErrors.NoSlotsFound(request.DateTime);
        }

        var appointment = await appointmentRepository
            .GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.Id);
        }

        if (appointment.IsApprovedByUser && appointment.IsApprovedByClinic)
        {
            return AppointmentErrors.AppointmentIsApproved(appointment.Id);
        }

        if (request.PatientId != appointment.PatientId)
        {
            var patient = await patientRepository
                .GetByIdAsync(request.PatientId, cancellationToken);
            if (patient is null)
            {
                return PatientErrors.NotFound(request.PatientId);
            }

            if (!patient.IsActive)
            {
                return PatientErrors.NotActive(request.PatientId);
            }
        }

        if (request.DoctorId != appointment.DoctorId)
        {
            var doctor = await doctorRepository
                .GetByIdAsync(request.PatientId, cancellationToken);
            if (doctor is null)
            {
                return DoctorErrors.NotFound(request.PatientId);
            }

            if (!doctor.IsActive)
            {
                return DoctorErrors.NotActive(request.PatientId);
            }
        }

        if (request.ServiceId != appointment.ServiceId)
        {
            var service = await serviceRepository
                .GetByIdAsync(request.ServiceId, cancellationToken);
            if (service is null)
            {
                return ServiceErrors.NotFound(request.ServiceId);
            }

            if (!service.IsActive)
            {
                return ServiceErrors.NotActive(request.ServiceId);
            }
        }


        var hasSameDate = slot.SlotStart == appointment.DateTimeStart &&
                          slot.SlotEnd == appointment.DateTimeEnd;

        var hasSamePatientDate = hasSameDate && request.PatientId == appointment.PatientId;
        if (!hasSamePatientDate)
        {
            var isPatientAvailable = await appointmentRepository
                .ExistsApprovedWithPatientAndDatetimeAsync
                    (request.PatientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors
                    .DateTimePatientTaken(request.PatientId, request.DateTime);
            }
        }

        var hasSameDoctorDate = hasSameDate && request.DoctorId == appointment.DoctorId;
        if (!hasSameDoctorDate)
        {
            var isDoctorAvailable = await appointmentRepository
                .ExistsApprovedWithDoctorAndDatetimeAsync
                    (request.DoctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors
                    .DateTimeDoctorTaken(request.DoctorId, request.DateTime);
            }
        }

        appointment.Update(request.DoctorId, request.PatientId,
            request.ServiceId, slot.SlotStart, slot.SlotEnd, 
            request.IsConfirmedByUser, request.IsConfirmedByClinic,
            request.Status);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}