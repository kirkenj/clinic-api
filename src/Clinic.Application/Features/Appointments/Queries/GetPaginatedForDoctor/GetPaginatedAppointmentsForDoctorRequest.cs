using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginatedForDoctor;

public class GetPaginatedAppointmentsForDoctorRequest : IRequest<Result<PaginatedCollection<AppointmentResponse>>>
{
    public uint DoctorId { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}