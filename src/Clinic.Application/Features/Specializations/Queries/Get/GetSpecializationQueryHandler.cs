using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.Get;

public sealed class GetSpecializationQueryHandler(ISpecializationRepository specializationRepository)
    : IRequestHandler<GetSpecializationQuery, Result<SpecializationResponse>>
{
    public async Task<Result<SpecializationResponse>> Handle(GetSpecializationQuery request,
        CancellationToken cancellationToken)
    {
        var specialization = await specializationRepository.GetByIdAsync(request.Id, cancellationToken);
        if (specialization is null)
        {
            return Result.Failure<SpecializationResponse>(
                SpecializationErrors.NotFound(request.Id));
        }

        return SpecializationResponse.FromSpecialization(specialization);
    }
}