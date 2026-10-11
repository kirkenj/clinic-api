using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetPaginated;

public class GetPaginatedDoctorsRequestHandler
    (IDoctorService doctorService,
    GetPaginatedDoctorsRequestValidator validator) : IRequestHandler<GetPaginatedDoctorsRequest,
    Result<PaginatedCollection<DoctorResponse>>>
{
    public async Task<Result<PaginatedCollection<DoctorResponse>>> Handle(GetPaginatedDoctorsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<DoctorResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        return await doctorService.GetPaginated(request.DoctorFilter, request.Page, 
            request.PageSize, cancellationToken);
    }
}