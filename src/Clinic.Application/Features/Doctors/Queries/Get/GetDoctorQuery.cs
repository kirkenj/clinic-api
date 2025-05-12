using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.Get;

public sealed class GetDoctorQuery : IRequest<Result<DoctorResponse>>
{
    public uint Id { get; set; }
}