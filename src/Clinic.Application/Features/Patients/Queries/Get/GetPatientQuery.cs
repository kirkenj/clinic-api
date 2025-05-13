using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get;

public sealed class GetPatientQuery : IRequest<Result<PatientResponse>>
{
    public uint Id { get; set; }
}