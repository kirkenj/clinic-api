using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Appointments;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Filters;
using Clinic.Domain.Repositories;

namespace Clinic.Application.Services;
public class AppointmentService
    (ISlotService slotService,
    IDoctorRepository doctorRepository,
    IPatientRepository patientRepository,
    IServiceRepository serviceRepository,
    IAppointmentRepository appointmentRepository,
    IOtpService otpService,
    IEmailService emailService,
    IDateTimeProvider dateTimeProvider)
    : IAppointmentService
{
    public async Task<Result<string>> AddByPatient(uint patientId, uint doctorId, uint serviceId,
        DateTime dateTime, CancellationToken cancellationToken)
    {
        var slot = slotService.GetSlot(dateTime.ToUniversalTime());
        if (slot == null)
        {
            return Result.Failure<string>(AppointmentErrors.NoSlotsFound(dateTime));
        }

        if (slot.SlotStart <= dateTimeProvider.UtcNow())
        {
            return Result.Failure<string>(AppointmentErrors.InvalidDate(dateTime));
        }

        var existingAppointment = await appointmentRepository.GetAsync(patientId, doctorId,
            serviceId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (existingAppointment is not null)
        {
            return Result.Failure<string>(AppointmentErrors.AlreadyExists(existingAppointment.Id));
        }

        var doctor = await doctorRepository
            .GetByIdAsync(doctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(doctorId));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(doctorId));
        }

        var patient = await patientRepository
            .GetByIdAsync(patientId, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(doctorId));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(doctorId));
        }

        var service = await serviceRepository
            .GetByIdAsync(serviceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(serviceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<string>(ServiceErrors.NotActive(serviceId));
        }

        var isDoctorSlotTaken = await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync
            (doctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isDoctorSlotTaken)
        {
            return Result.Failure<string>(AppointmentErrors.DateTimeDoctorTaken(doctorId, dateTime));
        }

        var isPatientSlotTaken = await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync
            (patientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isPatientSlotTaken)
        {
            return Result.Failure<string>(AppointmentErrors
                .DateTimePatientTaken(patientId, dateTime));
        }

        var appointment = Appointment.Create(doctorId, patientId, serviceId,
            slot.SlotStart, slot.SlotEnd, false, false,
            AppointmentStatus.WaitingUserConfirm);
        await appointmentRepository.AddAsync(appointment, cancellationToken);
        var otpKey = $"ConfirmAppointmentCreationByPatient{appointment.PatientId}:{appointment.Id}";
        var otpResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpResult.IsFailure)
        {
            return Result.Failure<string>(otpResult.Error);
        }

        await emailService.SendAsync(patient.Email, Constants.SubjectConfirmAppointment,
            EmailBodyBuilder.BuildConfirmAppointmentContent(service.Name, doctor.FirstName, doctor.LastName,
                service.Price,
                appointment.DateTimeStart, otpResult.Value), cancellationToken);
        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);

    }

    public async Task<Result> ApproveAppointmentCancellationByPatient(uint appointmentId, string otp, CancellationToken cancellationToken)
    {
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);

        var appointment = await appointmentRepository.GetByIdAsync
            (appointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(appointmentId);
        }

        if (appointment.Status != AppointmentStatus.Confirmed)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.Confirmed);
        }

        var otpKey = $"ConfirmAppointmentCancellationByPatient{appointment.PatientId}:{appointment.Id}";
        var otpValidationResul = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
        if (otpValidationResul.IsFailure)
        {
            return otpValidationResul.Error;
        }

        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(appointment.PatientId);
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

        await emailService.SendAsync(doctor.Email, Constants.SubjectAppointmentCancelled,
            EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name,
                patient.FirstName,
                patient.LastName, appointment.DateTimeStart),
            cancellationToken);
        return Result.Success();

    }

    public async Task<Result> ApproveByClinicAsync(uint appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(appointmentId);
        }

        if (appointment.Status != AppointmentStatus.WaitingClinicConfirm)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.WaitingClinicConfirm);
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
                Constants.SubjectNewAppointment,
                EmailBodyBuilder.BuildNewAppointmentForDoctorContent(service.Name, patient.FirstName, patient.LastName,
                    appointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }

    public async Task<Result> ApproveByPatientAsync(uint appointmentId, string otp, CancellationToken cancellationToken)
    {
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);

        var appointment = await appointmentRepository.GetByIdAsync
            (appointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(appointmentId);
        }

        if (appointment.IsApprovedByUser)
        {
            return AppointmentErrors.AppointmentIsApproved(appointment.Id);
        }

        if (appointment.Status != AppointmentStatus.WaitingUserConfirm)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.WaitingUserConfirm);
        }

        if (appointment.DateTimeStart.ToUniversalTime() <= dateTimeProvider.UtcNow())
        {
            return AppointmentErrors.InvalidDate(appointment.DateTimeStart);
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

        var otpKey = $"ConfirmAppointmentCreationByPatient{appointment.PatientId}:{appointment.Id}";
        var otpValidationResul = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
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

    public async Task<Result> CancelByClinicAsync(uint appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(appointmentId);
        }

        if (appointment.Status is not (
            AppointmentStatus.Confirmed or
            AppointmentStatus.WaitingClinicConfirm or
            AppointmentStatus.WaitingUserConfirm))
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.Confirmed,
                AppointmentStatus.WaitingClinicConfirm, AppointmentStatus.WaitingUserConfirm);
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, appointment.IsApprovedByUser,
            false, AppointmentStatus.CancelledByClinic);


        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        var patient = await patientRepository.GetByIdAsync(appointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(appointment.PatientId);
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

        await Task.WhenAll(
            emailService.SendAsync(patient.Email, Constants.SubjectAppointmentCancelled,
                EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name, doctor.FirstName,
                    doctor.LastName, appointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(doctor.Email, Constants.SubjectAppointmentCancelled,
                EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name, patient.FirstName,
                    patient.LastName, appointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }

    public async Task<Result<string>> CancelByPatientAsync(uint appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment == null)
        {
            return Result.Failure<string>(AppointmentErrors.NotFound(appointmentId));
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

    public async Task<Result<AppointmentResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(id, cancellationToken);
        if (appointment is null)
        {
            return Result.Failure<AppointmentResponse>(AppointmentErrors.NotFound(id));
        }

        return AppointmentResponse.FromAppointment(appointment);
    }

    public async Task<Result<IEnumerable<DateTime>>> GetDoctorsAvailableSlotsOnDate(DateTime date, uint doctorId, CancellationToken cancellationToken)
    {
        var slots = slotService.GetSlots(date);
        if (slots == null || slots.Count == 0)
        {
            return Result.Failure<IEnumerable<DateTime>>(AppointmentErrors.InvalidDate(date));
        }

        var doctor = await doctorRepository.GetByIdAsync(doctorId, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<IEnumerable<DateTime>>(DoctorErrors.NotFound(doctorId));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<IEnumerable<DateTime>>(DoctorErrors.NotActive(doctorId));
        }

        var currentDateTime = dateTimeProvider.UtcNow();
        var takenSlots = await appointmentRepository.GetDoctorsAppointmentsOnDate(
            doctorId, date, cancellationToken, AppointmentStatus.Confirmed);
        var freeSlots = slots.Where(slot => slot.SlotStart > currentDateTime &&
                !takenSlots.Any(appointment =>
                    slot.SlotStart < appointment.DateTimeEnd && slot.SlotEnd > appointment.DateTimeStart))
            .Select(s => s.SlotStart);
        return Result.Success(freeSlots);
    }

    public async Task<Result<PaginatedCollection<AppointmentResponse>>> GetPaginatedAsync(AppointmentFilter? appointmentFilter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var isDatePeriodInvalid = appointmentFilter is { DateTimeStart: not null, DateTimeEnd: not null } &&
            appointmentFilter.DateTimeStart.Value > appointmentFilter.DateTimeEnd.Value;
        if (isDatePeriodInvalid)
        {
            return Result.Failure<PaginatedCollection<AppointmentResponse>>(new Error("Bad request"));
        }

        var paginatedAppointments = await appointmentRepository.GetPaginatedAsync(appointmentFilter,
            page, pageSize, cancellationToken);

        var appointmentResponse = paginatedAppointments.Select(AppointmentResponse.FromAppointment);

        return Result.Success(appointmentResponse);
    }

    public async Task<Result> RescheduleAppointmentAsync(uint appointmentId, uint newDoctorId,
        DateTime newDateTime, CancellationToken cancellationToken)
    {
        var slot = slotService.GetSlot(newDateTime.ToUniversalTime());
        if (slot == null)
        {
            return AppointmentErrors.NoSlotsFound(newDateTime);
        }

        if (slot.SlotStart <= dateTimeProvider.UtcNow())
        {
            return AppointmentErrors.InvalidDate(newDateTime);
        }

        var oldAppointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (oldAppointment == null)
        {
            return AppointmentErrors.NotFound(appointmentId);
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

        var oldDoctor = await doctorRepository.GetByIdAsync(oldAppointment.DoctorId, cancellationToken);
        if (oldDoctor is null)
        {
            return DoctorErrors.NotFound(oldAppointment.DoctorId);
        }

        var newDoctor = oldAppointment.DoctorId != newDoctorId
            ? await doctorRepository.GetByIdAsync(newDoctorId, cancellationToken)
            : oldDoctor;
        if (newDoctor is null)
        {
            return DoctorErrors.NotFound(newDoctorId);
        }

        if (!newDoctor.IsActive)
        {
            return DoctorErrors.NotActive(newDoctorId);
        }

        var patient = await patientRepository.GetByIdAsync(oldAppointment.PatientId, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(oldAppointment.DoctorId);
        }

        if (!patient.IsActive)
        {
            return PatientErrors.NotActive(oldAppointment.DoctorId);
        }

        var service = await serviceRepository.GetByIdAsync(oldAppointment.ServiceId, cancellationToken);
        if (service is null)
        {
            return Result.Failure<string>(ServiceErrors.NotFound(oldAppointment.ServiceId));
        }

        if (!service.IsActive)
        {
            return Result.Failure<string>(ServiceErrors.NotActive(oldAppointment.ServiceId));
        }

        var isDoctorSlotTaken = await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync
            (newDoctorId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isDoctorSlotTaken)
        {
            return AppointmentErrors.DateTimeDoctorTaken(newDoctorId, newDateTime);
        }

        var isPatientSlotTaken = await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync
            (oldAppointment.PatientId, slot.SlotStart, slot.SlotEnd, cancellationToken);
        if (isPatientSlotTaken)
        {
            return AppointmentErrors.DateTimePatientTaken(oldAppointment.PatientId, newDateTime);
        }

        oldAppointment.Update(oldAppointment.DoctorId, oldAppointment.PatientId, oldAppointment.ServiceId,
            oldAppointment.DateTimeStart, oldAppointment.DateTimeEnd, oldAppointment.IsApprovedByUser,
            oldAppointment.IsApprovedByClinic, AppointmentStatus.Rescheduled);
        await appointmentRepository.UpdateAsync(oldAppointment, cancellationToken);
        var newAppointment = Appointment.Create(newDoctorId, oldAppointment.PatientId, oldAppointment.ServiceId,
            slot.SlotStart, slot.SlotEnd, true, true, AppointmentStatus.Confirmed);
        await appointmentRepository.AddAsync(newAppointment, cancellationToken);

        await Task.WhenAll(
            emailService.SendAsync(oldDoctor.Email, Constants.SubjectAppointmentCancelled,
                EmailBodyBuilder.BuildCancelledAppointmentContent(service.Name,
                    patient.FirstName, patient.LastName, oldAppointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(newDoctor.Email, Constants.SubjectNewAppointment,
                EmailBodyBuilder.BuildNewAppointmentForDoctorContent(service.Name,
                    patient.FirstName, patient.LastName, newAppointment.DateTimeStart),
                cancellationToken),
            emailService.SendAsync(patient.Email, Constants.SubjectAppointmentRescheduled,
                EmailBodyBuilder.BuildAppointmentRescheduledContent(service.Name,
                    oldDoctor.FirstName, oldDoctor.LastName, oldAppointment.DateTimeStart,
                    newDoctor.FirstName, newDoctor.LastName, newAppointment.DateTimeStart),
                cancellationToken));
        return Result.Success();
    }

    public async Task<Result> SendOtpToCreateAppointment(uint appointmentId, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(appointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(appointmentId);
        }

        if (appointment.Status != AppointmentStatus.WaitingUserConfirm)
        {
            return AppointmentErrors.InvalidStatus(appointment.Status, AppointmentStatus.WaitingUserConfirm);
        }

        if (appointment.DateTimeStart <= dateTimeProvider.UtcNow())
        {
            return AppointmentErrors.InvalidDate(appointment.DateTimeStart);
        }

        var otpKey = $"ConfirmAppointmentCreationByPatient{appointment.PatientId}:{appointment.Id}";
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

        await emailService.SendAsync(patient.Email, Constants.SubjectConfirmAppointment,
            EmailBodyBuilder.BuildConfirmAppointmentContent(service.Name, doctor.FirstName,
                doctor.LastName, service.Price, appointment.DateTimeStart, otpResult.Value),
            cancellationToken);
        return Result.Success();
    }
}