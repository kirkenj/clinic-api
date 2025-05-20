using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetPaginated;

public class GetPaginatedSpecializationsRequest : IRequest<Result<PaginatedCollection<SpecializationResponse>>>
{
    public int Page { get; set; }
    public int PageSize { get; set; }
}