using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetAll;

public sealed class GetAllSpecializationsQueryHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<GetAllSpecializationsQuery, Result<IEnumerable<SpecializationResponse>>>
{
    public async Task<Result<IEnumerable<SpecializationResponse>>> Handle(GetAllSpecializationsQuery request,
        CancellationToken cancellationToken)
    {
        var specializations = await specializationRepository.GetAllAsync(cancellationToken);
        return Result.Success(SpecializationResponse.FromSpecializationCollection(specializations));
    }
}