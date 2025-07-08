using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;

public class GetPaginatedAppointmentResultsRequest : IRequest<Result<PaginatedCollection<AppointmentResultResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
    public AppointmentResultFilter? AppointmentResultFilter { get; set; }
}