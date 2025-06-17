using Clinic.Application.Responses.AppointmentResults;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Queries.GetPaginated;

public class GetPaginatedAppointmentResultsRequest : IRequest<Result<PaginatedCollection<AppointmentResultResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}