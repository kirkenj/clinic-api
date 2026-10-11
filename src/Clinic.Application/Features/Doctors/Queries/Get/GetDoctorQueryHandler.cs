using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.Get;

public sealed class GetDoctorQueryHandler(IDoctorService doctorService)
    : IRequestHandler<GetDoctorQuery, Result<DoctorResponse>>
{
    public async Task<Result<DoctorResponse>> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
    {
        return await doctorService.GetByIdAsync(request.Id, cancellationToken);
    }
}