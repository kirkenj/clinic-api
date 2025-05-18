using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetPaginated;

public class GetPaginatedReceptionistsRequest : IRequest<Result<PaginatedCollection<ReceptionistResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}