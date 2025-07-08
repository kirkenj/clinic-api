using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetCurrent;

public sealed class GetCurrentDoctorQuery : IRequest<Result<DoctorResponse>>{ }