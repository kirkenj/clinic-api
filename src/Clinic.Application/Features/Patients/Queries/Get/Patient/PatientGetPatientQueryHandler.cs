using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get.Patient;

public sealed class PatientGetPatientQueryHandler
    (IPatientService patientService,
    IUserAccessService userAccessService)
    : IRequestHandler<PatientGetPatientQuery, Result<PatientResponse>>
{
    public async Task<Result<PatientResponse>> Handle(PatientGetPatientQuery request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            [RoleType.Patient], cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<PatientResponse>(userAccessResult.Error);
        }

        return await patientService.GetByIdAsync(userAccessResult.Value.Id, cancellationToken);
    }
}