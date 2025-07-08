using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetAvailableSlots.cs;
public sealed class GetDoctorsAvailableSlotsOnDateQuery : IRequest<Result<IEnumerable<DateTime>>>
{
    public DateTime Date { get; set; }
    public uint DoctorId { get; set; }
}
