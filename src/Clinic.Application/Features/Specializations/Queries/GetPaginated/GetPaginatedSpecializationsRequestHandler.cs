using Clinic.Application.Extensions;
using Clinic.Application.Responses.Specializations;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Specializations.Queries.GetPaginated;

public class GetPaginatedSpecializationsRequestHandler
    (ISpecializationRepository specializationRepository,
    GetPaginatedSpecializationsRequestValidator validator)
    : IRequestHandler<GetPaginatedSpecializationsRequest, Result<PaginatedCollection<SpecializationResponse>>>
{
    public async Task<Result<PaginatedCollection<SpecializationResponse>>> Handle(
        GetPaginatedSpecializationsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<SpecializationResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedSpecializations =
            await specializationRepository.GetPaginatedAsync(request.Filter, request.Page, request.PageSize, cancellationToken);
        var specializationResponse = paginatedSpecializations.Select(SpecializationResponse.FromSpecialization);
        return Result.Success(specializationResponse);
    }
}