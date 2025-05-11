using Clinic.Application.Responses.Doctor;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctor.Queries.Get;

public sealed record GetDoctorQuery(uint Id) : IRequest<Result<DoctorResponse>>
{ }