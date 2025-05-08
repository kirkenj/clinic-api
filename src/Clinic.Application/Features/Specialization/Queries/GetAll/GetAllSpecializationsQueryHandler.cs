using Clinic.Application.Dtos.Specialization;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Specialization;
using MediatR;

namespace Clinic.Application.Features.Specialization.Queries.GetAll;
public sealed class GetAllSpecializationsQueryHandler(ISpecializationRepository specializationRepository) 
    : IRequestHandler<GetAllSpecializationsQuery, Result<IEnumerable<SpecializationResponse>>>
{
    public async Task<Result<IEnumerable<SpecializationResponse>>> Handle(GetAllSpecializationsQuery request, CancellationToken cancellationToken)
    {
        var specializations = await specializationRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(SpecializationResponse.FromSpecializationCollection(specializations));
    }
}
