using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.Get;

public sealed record GetDoctorQuery(uint Id) : IRequest<Result<DoctorResponse>>
{ }