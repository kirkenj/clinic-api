using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.GetAll;

public sealed class GetAllPatientsQueryHandler(IPatientRepository patientRepository)
    : IRequestHandler<GetAllPatientsQuery, Result<IEnumerable<PatientResponse>>>
{
    public async Task<Result<IEnumerable<PatientResponse>>> Handle(GetAllPatientsQuery request,
        CancellationToken cancellationToken)
    {
        var patients = await patientRepository.GetAllAsync(cancellationToken);
        return Result.Success(PatientResponse.FromPatientCollection(patients));
    }
}