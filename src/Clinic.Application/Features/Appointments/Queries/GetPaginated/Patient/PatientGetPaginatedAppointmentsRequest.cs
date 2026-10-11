using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated.Patient;

public class PatientGetPaginatedAppointmentsRequest : IRequest<Result<PaginatedCollection<AppointmentResponse>>>
{
    public PatientAppointmentFilter? AppointmentFilter { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}