using System.Text.Json;
using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Appointments;
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
    IOtpService otpService,
    INotificationService notificationService,
    ISlotService slotService)
    : IRequestHandler<AddAppointmentCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddAppointmentCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var slot = slotService.GetSlot(request.DateTime);
        if (slot == null)
        {
            return Result.Failure<string>(AppointmentErrors.NoSlotsFound(request.DateTime));
        }

        if (slot.SlotStart <= DateTime.UtcNow)
        {
            return Result.Failure<string>(AppointmentErrors.InvalidDate(request.DateTime));
        }

        var existingAppointment = await appointmentRepository.GetAsync(request.PatientId, request.DoctorId,
            request.ServiceId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (existingAppointment is not null)
        {
            return Result.Failure<string>(AppointmentErrors.AlreadyExists(existingAppointment.Id));
        }

        var doctor = await doctorRepository
            .GetByIdAsync(request.DoctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(request.DoctorId));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(request.DoctorId));
        }

        var patient = await patientRepository
            .GetByIdAsync(request.PatientId, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(request.DoctorId));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(request.DoctorId));
        }

        var service = await serviceRepository
            .GetByIdAsync(request.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(request.ServiceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<string>(ServiceErrors.NotActive(request.ServiceId));
        }

        var isDoctorSlotTaken = await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync
            (request.DoctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isDoctorSlotTaken)
        {
            return Result.Failure<string>(AppointmentErrors.DateTimeDoctorTaken
                (request.DoctorId, request.DateTime));
        }

        var isPatientSlotTaken = await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync
            (request.PatientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isPatientSlotTaken)
        {
            return Result.Failure<string>(AppointmentErrors
                .DateTimePatientTaken(request.PatientId, request.DateTime));
        }

        var appointment = Appointment.Create(request.DoctorId, request.PatientId, request.ServiceId,
            slot.SlotStart, slot.SlotEnd, false, false,
            AppointmentStatus.WaitingUserConfirm);
        await appointmentRepository.AddAsync(appointment, cancellationToken);
        var otpKey = string.Format(ApplicationResources.ConfirmAppointmentCreationByPatientOtpKeyFormat,
            appointment.PatientId, appointment.Id);
        var otpResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure<string>(otpResult.Error);
        }

        var emailResponse = new EmailAppointmentResponse(doctor.FirstName + " " + doctor.LastName,
            service.Name, service.Price, appointment.DateTimeStart);
        var body = string.Format(ApplicationResources.ConfirmAppointmentCreationByPatientNotificationSubjectBody, 
            JsonSerializer.Serialize(emailResponse), otpResult.Value);
        await notificationService.NotifyAsync(patient, 
            ApplicationResources.ConfirmAppointmentCreationByPatientNotificationSubject,
            body, cancellationToken);
        return Result.Success(ApplicationResources.CheckEmailForFurtherDetailsResponse);
    }
}