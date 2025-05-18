using Clinic.Application.Extensions;
using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Doctors.Queries.GetPaginated;

public class GetPaginatedDoctorsRequestHandler(ILogger<GetPaginatedDoctorsRequestHandler> logger,
    IDoctorRepository doctorRepository,
    GetPaginatedDoctorsRequestValidator validator) : IRequestHandler<GetPaginatedDoctorsRequest,
    Result<PaginatedCollection<DoctorResponse>>>
{
    public async Task<Result<PaginatedCollection<DoctorResponse>>> Handle(GetPaginatedDoctorsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedDoctorsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<DoctorResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedDoctors =
            await doctorRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var doctorResponse = paginatedDoctors.Select(DoctorResponse.FromDoctor);
        logger.LogInformation("PaginatedDoctorsResponse: {@response}", doctorResponse);
        return Result.Success(doctorResponse);
    }
}