using Clinic.Application.Responses.Appointments;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries.GetPaginated;

public class GetPaginatedAppointmentsRequest : IRequest<Result<PaginatedCollection<AppointmentResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public AppointmentFilter? AppointmentFilter { get; set; }
}