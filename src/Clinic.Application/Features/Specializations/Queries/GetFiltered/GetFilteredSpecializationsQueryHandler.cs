using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetFiltered;

public sealed class GetFilteredSpecializationsQueryHandler : IRequestHandler<GetFilteredSpecializationsQuery, Result<PaginatedCollection<SpecializationResponse>>>
{
    public Task<Result<PaginatedCollection<SpecializationResponse>>> Handle(GetFilteredSpecializationsQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}