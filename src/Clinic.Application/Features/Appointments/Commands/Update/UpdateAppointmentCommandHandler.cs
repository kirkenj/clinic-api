using Clinic.Application.Extensions;
using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Features.Appointments.Commands.Update;

public sealed class UpdateAppointmentCommandHandler
    (IAppointmentRepository appointmentRepository,
    IPatientRepository patientRepository,
    IDoctorRepository doctorRepository,
    IServiceRepository serviceRepository,
    UpdateAppointmentCommandValidator validator,
    IOptions<AppointmentsOptions> options)
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

        request.DateTime = request.DateTime
            .NormalizeDateTimeToMinutePeriod(options.Value.TimeSlotSizeMinutes);

        var requestDayOfWeek = request.DateTime.DayOfWeek;
        if (requestDayOfWeek < options.Value.WorkWeekDayStart || 
            requestDayOfWeek > options.Value.WorkWeekDayEnd)
        {
            return AppointmentErrors.InvalidDayOfWeek
                (requestDayOfWeek, options.Value.WorkWeekDayStart, options.Value.WorkWeekDayEnd);
        }

        var requestTimeOfDay = request.DateTime.TimeOfDay;
        if (requestTimeOfDay < options.Value.WorkTimeStart || 
            requestTimeOfDay > options.Value.WorkTimeStart)
        {
            return AppointmentErrors.InvalidTimeOfDay
                (requestTimeOfDay, options.Value.WorkTimeStart, options.Value.WorkTimeEnd);
        }

        var appointment = await appointmentRepository
            .GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.Id);
        }

        if (appointment.IsBooked && appointment.IsApproved)
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

        var hasSamePatientDate = request.DateTime == appointment.DateTime && 
                                 request.PatientId == appointment.PatientId;
        if (!hasSamePatientDate)
        {
            var isPatientAvailable = await appointmentRepository
                .ExistsApprovedWithPatientAndDatetimeAsync
                    (request.PatientId, request.DateTime, cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors
                    .DateTimePatientTaken(request.PatientId, request.DateTime);
            }
        }

        var hasSameDoctorDate = request.DateTime == appointment.DateTime && 
                                request.DoctorId == appointment.DoctorId;
        if (!hasSameDoctorDate)
        {
            var isDoctorAvailable = await appointmentRepository
                .ExistsApprovedWithDoctorAndDatetimeAsync
                    (request.DoctorId, request.DateTime, cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors
                    .DateTimeDoctorTaken(request.DoctorId, request.DateTime);
            }
        }

        appointment.Update(request.DoctorId, request.PatientId, 
            request.ServiceId, request.DateTime);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}