using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.GetAll;

public sealed class GetAllPatientsQuery : IRequest<Result<IEnumerable<PatientResponse>>>
{
}