using Clinic.Application.Dtos.Speciallization;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Speciallization;
using MediatR;

namespace Clinic.Application.Features.Speciallization.Queries.Get;

public sealed class GetSpeciallizationQueryHandler(ISpeciallizationRepository speciallizationRepository) : IRequestHandler<GetSpeciallizationQuery, Result<SpeciallizationDto>>
{
    public async Task<Result<SpeciallizationDto>> Handle(GetSpeciallizationQuery request, CancellationToken cancellationToken)
    {
        var speciallization = await speciallizationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (speciallization == null)
        {
            return Result.Failure<SpeciallizationDto>(
                SpeciallizationErrors.NotFound(request.Id));
        }

        return SpeciallizationDto.FromSpeciallization(speciallization);
    }
}