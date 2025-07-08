using Clinic.Application.Responses.Patients;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Patients.Queries.Get.Patient;

public sealed class PatientGetPatientQuery : IRequest<Result<PatientResponse>> { }