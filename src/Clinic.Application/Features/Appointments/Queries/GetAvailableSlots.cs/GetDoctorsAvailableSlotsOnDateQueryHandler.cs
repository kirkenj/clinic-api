using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetAvailableSlots.cs;
public sealed class GetDoctorsAvailableSlotsOnDateQueryHandler
    (IAppointmentService appointmentService,
    GetDoctorsAvailableSlotsOnDateQueryValidator validator)
    : IRequestHandler<GetDoctorsAvailableSlotsOnDateQuery, Result<IEnumerable<DateTime>>>
{
    public async Task<Result<IEnumerable<DateTime>>> Handle(GetDoctorsAvailableSlotsOnDateQuery request, CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<IEnumerable<DateTime>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await appointmentService.GetDoctorsAvailableSlotsOnDate(request.Date,
            request.DoctorId, cancellationToken);
    }
}
