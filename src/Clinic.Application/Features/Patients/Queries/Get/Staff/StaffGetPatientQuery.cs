using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get.Staff;

public sealed class StaffGetPatientQuery : IRequest<Result<PatientResponse>>
{
    public uint Id { get; set; }
}