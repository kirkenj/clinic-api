using Clinic.Application.Dtos.Speciallization;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Queries.Get;

public sealed class GetSpeciallizationQueryHandler(ISpeciallizationRepository speciallizationRepository) : IRequestHandler<GetSpeciallizationQuery, Result<SpeciallizationResponse>>
{
    public async Task<Result<SpeciallizationResponse>> Handle(GetSpeciallizationQuery request, CancellationToken cancellationToken)
    {
        var speciallization = await speciallizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciallization == null)
        {
            return Result.Failure<SpeciallizationResponse>(
                SpeciallizationErrors.NotFound(request.Id));
        }

        return SpeciallizationResponse.FromSpeciallization(speciallization);
    }
}