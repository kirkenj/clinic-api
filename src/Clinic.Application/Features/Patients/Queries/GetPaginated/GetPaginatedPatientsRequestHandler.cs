using Clinic.Application.Extensions;
using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Clinic.Application.Features.Patients.Queries.GetPaginated;

public class GetPaginatedPatientsRequestHandler(ILogger<GetPaginatedPatientsRequestHandler> logger,
    IPatientRepository patientRepository,
    GetPaginatedPatientsRequestValidator validator) : IRequestHandler<GetPaginatedPatientsRequest,
    Result<PaginatedCollection<PatientResponse>>>
{
    public async Task<Result<PaginatedCollection<PatientResponse>>> Handle(GetPaginatedPatientsRequest request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("GetPaginatedPatientsRequest: {@request}", request);
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            logger.LogWarning("Validation failed: {@validationResult}", validationResult);
            return Result.Failure<PaginatedCollection<PatientResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var paginatedPatients =
            await patientRepository.GetPaginatedAsync(request.Page, request.PageSize, cancellationToken);
        var patientResponse = paginatedPatients.Select(PatientResponse.FromPatient);
        logger.LogInformation("PaginatedPatientsResponse: {@response}", patientResponse);
        return Result.Success(patientResponse);
    }
}