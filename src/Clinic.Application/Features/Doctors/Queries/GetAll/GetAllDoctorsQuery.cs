using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetAll;

public sealed class GetAllDoctorsQuery : IRequest<Result<IEnumerable<DoctorResponse>>>
{
}