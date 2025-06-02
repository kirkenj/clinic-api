using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetAvailableSlotsForServiceOnDate;

public sealed class GetAvailableSlotsForServiceOnDateRequest : IRequest<Result<IEnumerable<SlotAndAvailableDoctors>>>
{
    public uint ServiceId { get; set; }
    public DateTime Date { get; set; }
}