using Clinic.Application.Extensions;
using Clinic.Application.Responses.Receptionists;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Queries.GetPaginated;

public class GetPaginatedReceptionistsRequestHandler
    (IReceptionistRepository receptionistRepository,
    GetPaginatedReceptionistsRequestValidator validator) 
    : IRequestHandler<GetPaginatedReceptionistsRequest,Result<PaginatedCollection<ReceptionistResponse>>>
{
    public async Task<Result<PaginatedCollection<ReceptionistResponse>>> Handle(
        GetPaginatedReceptionistsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<ReceptionistResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedReceptionists =
            await receptionistRepository.GetPaginatedAsync(request.ReceptionistFilter, request.Page, request.PageSize, cancellationToken);
        var receptionistResponse = paginatedReceptionists.Select(ReceptionistResponse.FromReceptionist);
        return Result.Success(receptionistResponse);
    }
}