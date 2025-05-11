using Clinic.Application.Responses.Doctor;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctor.Queries.GetAll;
public sealed record GetAllDoctorsQuery : IRequest<Result<IEnumerable<DoctorResponse>>>
{ }