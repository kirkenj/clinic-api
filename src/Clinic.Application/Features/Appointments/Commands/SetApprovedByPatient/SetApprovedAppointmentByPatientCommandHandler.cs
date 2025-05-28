using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByPatient;

public class
    SetApprovedAppointmentByPatientCommandHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<SetApprovedAppointmentByPatientCommand, Result>
{
    public async Task<Result> Handle(SetApprovedAppointmentByPatientCommand request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync
            (request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.IsBooked == request.IsBooked)
        {
            return Result.Success();
        }

        if (request.IsBooked)
        {
            if (!appointment.IsApproved)
            {
                return AppointmentErrors
                    .AppointmentIsNotApprovedByClinic(request.AppointmentId);
            }

            var isDoctorAvailable =
                await appointmentRepository
                    .ExistsApprovedWithDoctorAndDatetimeAsync
                        (appointment.DoctorId, appointment.DateTime, cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors
                    .DateTimeDoctorTaken
                        (appointment.DoctorId, appointment.DateTime);
            }

            var isPatientAvailable =
                await appointmentRepository
                    .ExistsApprovedWithPatientAndDatetimeAsync
                        (appointment.PatientId, appointment.DateTime, cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors
                    .DateTimePatientTaken(appointment.PatientId, appointment.DateTime);
            }
        }

        appointment.SetBooked(request.IsBooked);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);
        return Result.Success();
    }
}