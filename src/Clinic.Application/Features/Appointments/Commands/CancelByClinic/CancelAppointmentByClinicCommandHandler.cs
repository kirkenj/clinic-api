using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.CancelByClinic;

public sealed class CancelAppointmentByClinicCommandHandler(
    IAppointmentRepository appointmentRepository,
    CancelAppointmentByClinicCommandValidator validator)
    : IRequestHandler<CancelAppointmentByClinicCommand, Result>
{
    public async Task<Result> Handle(CancelAppointmentByClinicCommand request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError
                (validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var appointment = await appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.Id);
        }

        appointment.Update(appointment.DoctorId, appointment.PatientId, appointment.ServiceId,
            appointment.DateTimeStart, appointment.DateTimeEnd, appointment.IsApprovedByUser,
            false, AppointmentStatus.CancelledByClinic);

        await appointmentRepository.DeleteAsync(appointment, cancellationToken);
        return Result.Success();
    }
}