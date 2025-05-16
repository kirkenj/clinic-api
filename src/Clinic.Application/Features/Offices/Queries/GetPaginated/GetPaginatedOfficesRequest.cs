using Clinic.Application.Responses.Offices;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;

public class GetPaginatedOfficesRequest : IRequest<Result<PaginatedCollection<OfficeResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}