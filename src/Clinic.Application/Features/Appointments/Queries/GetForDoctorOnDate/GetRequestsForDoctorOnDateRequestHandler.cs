using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetForDoctorOnDate;

public sealed class
    GetRequestsForDoctorOnDateRequestHandler(
        IAppointmentRepository appointmentRepository,
        GetRequestsForDoctorOnDateRequestValidator validator) : IRequestHandler<GetRequestsForDoctorOnDateRequest,
    Result<IEnumerable<AppointmentResponse>>>
{
    public async Task<Result<IEnumerable<AppointmentResponse>>> Handle(GetRequestsForDoctorOnDateRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<IEnumerable<AppointmentResponse>>
                (Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var appointments = await appointmentRepository
            .GetDoctorsAppointmentsOnDate(request.DoctorId, request.Date, cancellationToken);
        var appointmentResponse = appointments
            .Select(AppointmentResponse.FromAppointment);
        return Result.Success(appointmentResponse);
    }
}