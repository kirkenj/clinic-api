using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.SetApprovedByClinic;

public class
    SetApprovedAppointmentByClinicCommandHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<SetApprovedAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(SetApprovedAppointmentByClinicCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.AppointmentId, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.AppointmentId);
        }

        if (appointment.IsApproved == request.IsApproved)
        {
            return Result.Success();
        }

        if (request.IsApproved)
        {
            var isDoctorAvailable =
                await appointmentRepository.ExistsApprovedWithDoctorAndDatetimeAsync(appointment.DoctorId,
                    appointment.DateTime,
                    cancellationToken);
            if (isDoctorAvailable)
            {
                return AppointmentErrors.DateTimeDoctorTaken(appointment.DoctorId, appointment.DateTime);
            }

            var isPatientAvailable =
                await appointmentRepository.ExistsApprovedWithPatientAndDatetimeAsync(appointment.PatientId,
                    appointment.DateTime,
                    cancellationToken);
            if (isPatientAvailable)
            {
                return AppointmentErrors.DateTimePatientTaken(appointment.PatientId, appointment.DateTime);
            }
        }

        appointment.SetApproved(request.IsApproved);
        await appointmentRepository.UpdateAsync(appointment, cancellationToken);

        return Result.Success();
    }
}