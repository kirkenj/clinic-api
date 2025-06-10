using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetForDoctorOnDate;

public sealed class GetRequestsForDoctorOnDateRequest : IRequest<Result<IEnumerable<AppointmentResponse>>>
{
    public uint DoctorId { get; set; }
    public DateTime Date { get; set; }
    public AppointmentStatus? AppointmentStatus { get; set; }
}