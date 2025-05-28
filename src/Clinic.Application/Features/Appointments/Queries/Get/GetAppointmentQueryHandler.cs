using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.Get;

public sealed class GetAppointmentQueryHandler(IAppointmentRepository appointmentRepository)
    : IRequestHandler<GetAppointmentQuery, Result<AppointmentResponse>>
{
    public async Task<Result<AppointmentResponse>> Handle(GetAppointmentQuery request,
        CancellationToken cancellationToken)
    {
        var appointment = await appointmentRepository.GetByIdAsync(request.Id, cancellationToken);
        if (appointment is null)
        {
            return Result.Failure<AppointmentResponse>(
                AppointmentErrors.NotFound(request.Id));
        }

        return AppointmentResponse.FromAppointment(appointment);
    }
}