using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Filters;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetPaginated;

public class GetPaginatedSpecializationsRequest : IRequest<Result<PaginatedCollection<SpecializationResponse>>>
{
    public SpecializationFilter? Filter { get; set; }
    public int Page { get; set; }
    public int PageSize { get; set; }
}