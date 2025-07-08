using Clinic.Application.Abstractions;
using Clinic.Application.Extensions;
using Clinic.Application.Responses.Patients;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.GetPaginated;

public class GetPaginatedPatientsRequestHandler
    (IPatientService patientService,
    IUserAccessService userAccessService,
    GetPaginatedPatientsRequestValidator validator)
    : IRequestHandler<GetPaginatedPatientsRequest, Result<PaginatedCollection<PatientResponse>>>
{
    public async Task<Result<PaginatedCollection<PatientResponse>>> Handle(GetPaginatedPatientsRequest request,
        CancellationToken cancellationToken)
    {
        var validationResult = validator.Validate(request);
        if (!validationResult.IsValid)
        {
            return Result.Failure<PaginatedCollection<PatientResponse>>(
                Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.StaffRoles, cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<PaginatedCollection<PatientResponse>>(
                userAccessResult.Error);
        }

        return await patientService.GetPaginatedAsync(request.PatientFilter, request.Page,
            request.PageSize, cancellationToken);
    }
}