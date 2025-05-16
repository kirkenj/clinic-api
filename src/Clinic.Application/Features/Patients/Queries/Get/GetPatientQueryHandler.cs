using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get;

public sealed class GetPatientQueryHandler(IPatientRepository patientRepository)
    : IRequestHandler<GetPatientQuery, Result<PatientResponse>>
{
    public async Task<Result<PatientResponse>> Handle(GetPatientQuery request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<PatientResponse>(
                PatientErrors.NotFound(request.Id));
        }

        return PatientResponse.FromPatient(patient);
    }
}