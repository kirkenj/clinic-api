using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByPatient;

public class
    SetApprovedAppointmentByPatientCommandHandler
    (IAppointmentRepository appointmentRepository, 
     IOptions<AppointmentsOptions> options)
    : IRequestHandler<SetApprovedAppointmentByPatientCommand, Result>
{
    public async Task<Result> Handle(SetApprovedAppointmentByPatientCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException("Сделай отп для сего");

        var appointment = await appointmentRepository.GetByIdAsync
            (request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.IsApprovedByUser == request.IsBooked)
        {
            return Result.Success();
        }

        if (request.IsBooked)
        {
            if (!appointment.IsApprovedByClinic)
            {
                return AppointmentErrors
                    .AppointmentIsNotApprovedByClinic(request.AppointmentId);
            }

            var isDoctorAvailable =
                await appointmentRepository
                    .ExistsApprovedWithDoctorAndDatetimeAsync
                        (appointment.DoctorId, appointment.DateTimeStart, appointment.DateTimeEnd, cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors
                    .DateTimeDoctorTaken
                        (appointment.DoctorId, appointment.DateTimeStart);
            }

            var isPatientAvailable =
                await appointmentRepository
                    .ExistsApprovedWithPatientAndDatetimeAsync
                        (appointment.PatientId, appointment.DateTimeStart, appointment.DateTimeEnd, cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors
                    .DateTimePatientTaken(appointment.PatientId, appointment.DateTimeStart);
            }
        }


        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, true,
            appointment.IsApprovedByClinic, AppointmentStatus.WaitingClinicConfirm); 
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}