using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands.Delete;

public sealed class DeleteAppointmentCommandHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<DeleteAppointmentCommand, Result>
{
    public async Task<Result> Handle(DeleteAppointmentCommand request, CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return AppointmentErrors.NotFound(request.Id);
        }

        if (appointment.IsBooked && appointment.IsApproved)
        {
            return AppointmentErrors.AppointmentIsApproved(request.Id);
        }

        await appointmentRepository.DeleteAsync(appointment, cancellationToken);
        return Result.Success();
    }
}