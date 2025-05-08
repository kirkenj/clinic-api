using Clinic.Application.Dtos.Speciallization;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Queries.GetAll;
public sealed class GetAllSpeciallizationsQueryHandler(ISpeciallizationRepository speciallizationRepository) : IRequestHandler<GetAllSpeciallizationsQuery, Result<IEnumerable<SpeciallizationResponse>>>
{
    public async Task<Result<IEnumerable<SpeciallizationResponse>>> Handle(GetAllSpeciallizationsQuery request, CancellationToken cancellationToken)
    {
        var speciallizations = await speciallizationRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(SpeciallizationResponse.FromSpeciallizationCollection(speciallizations));
    }
}
