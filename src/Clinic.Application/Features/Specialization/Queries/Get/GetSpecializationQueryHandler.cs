using Clinic.Application.Dtos.Specialization;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Specialization;
using MediatR;

namespace Clinic.Application.Features.Specialization.Queries.Get;

public sealed class GetSpecializationQueryHandler(ISpecializationRepository specializationRepository) 
    : IRequestHandler<GetSpecializationQuery, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(GetSpecializationQuery request, CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization == null)
        {
            return Result.Failure<SpecializationResponse>(
                SpecializationErrors.NotFound(request.Id));
        }

        return SpecializationResponse.FromSpecialization(specialization);
    }
}