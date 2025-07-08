using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Patients;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get.Staff;

public sealed class StaffGetPatientQueryHandler
    (IPatientService patientService,
    IUserAccessService userAccessService)
    : IRequestHandler<StaffGetPatientQuery, Result<PatientResponse>>
{
    public async Task<Result<PatientResponse>> Handle(StaffGetPatientQuery request, CancellationToken cancellationToken)
    {
        var userAccessResult = await userAccessService.ValidateUserAccessAsync(
            Constants.StaffRoles, cancellationToken);
        if (userAccessResult.IsFailure)
        {
            return Result.Failure<PatientResponse>(userAccessResult.Error);
        }

        return await patientService.GetByIdAsync(request.Id, cancellationToken);
    }
}