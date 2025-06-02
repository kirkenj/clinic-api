using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByClinic;

public class
    SetApprovedAppointmentByClinicCommandHandler
    (IAppointmentRepository appointmentRepository,
     IOptions<AppointmentsOptions> options)
    : IRequestHandler<SetApprovedAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(SetApprovedAppointmentByClinicCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.IsApprovedByClinic == request.IsApproved)
        {
            return Result.Success();
        }

        if (!appointment.IsApprovedByUser)
        {
            return AppointmentErrors.AppointmentIsNotApprovedByUser(appointment.PatientId);
        }

        if (request.IsApproved)
        {
            var dateTimeEnd = appointment.DateTimeStart.AddMinutes(options.Value.TimeSlotSizeMinutes);

            var isDoctorAvailable =
                await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync(appointment.DoctorId,
                    appointment.DateTimeStart,
                    dateTimeEnd,
                    cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors.DateTimeDoctorTaken(appointment.DoctorId, appointment.DateTimeStart);
            }

            var isPatientAvailable =
                await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync(appointment.PatientId,
                    appointment.DateTimeStart,
                    dateTimeEnd,
                    cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors.DateTimePatientTaken(appointment.PatientId, appointment.DateTimeStart);
            }
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, appointment.IsApprovedByUser, 
            true, AppointmentStatus.Confirmed);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        return Result.Success();
    }
}