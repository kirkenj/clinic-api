using Clinic.Application.Services.SlotService;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Add;

public sealed class AddAppointmentCommandHandler(
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository,
    IServiceRepository serviceRepository,
    IAppointmentRepository appointmentRepository,
    AddAppointmentCommandValidator validator,
    ISlotService slotService)
    : IRequestHandler<AddAppointmentCommand, Result>
{
    public async Task<Result> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
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

        var doctor = await doctorRepository
            .GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(request.DoctorId);
        }

        if (!doctor.IsActive)
        {
            return DoctorErrors.NotActive(request.DoctorId);
        }

        var patient = await patientRepository
            .GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.DoctorId);
        }

        if (!patient.IsActive)
        {
            return PatientErrors.NotActive(request.DoctorId);
        }

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

        var isDoctorAvailable = await appointmentRepository
            .ExistsApprovedWithDoctorAndDatetimeAsync
                (request.DoctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isDoctorAvailable)
        {
            return AppointmentErrors.DateTimeDoctorTaken
                (request.DoctorId, request.DateTime);
        }

        var isPatientAvailable = await appointmentRepository
            .ExistsApprovedWithPatientAndDatetimeAsync
                (request.PatientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isPatientAvailable)
        {
            return AppointmentErrors
                .DateTimePatientTaken(request.PatientId, request.DateTime);
        }

        var appointment = Appointment.Create
            (request.DoctorId, request.PatientId, request.ServiceId, slot.SlotStart, slot.SlotEnd,
                false, false, AppointmentStatus.WaitingUserConfirm);
        await appointmentRepository.AddAsync(appointment, cancellationToken);





        return Result.Success();
    }
}