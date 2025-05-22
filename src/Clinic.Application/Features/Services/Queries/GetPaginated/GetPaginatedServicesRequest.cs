using Clinic.Application.Responses.Services;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Services.Queries.GetPaginated;

public class GetPaginatedServicesRequest : IRequest<Result<PaginatedCollection<ServiceResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}