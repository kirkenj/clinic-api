using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.GetAll;

public sealed class GetAllDoctorsQueryHandler(IDoctorRepository doctorRepository)
    : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<DoctorResponse>>>
{
    public async Task<Result<IEnumerable<DoctorResponse>>> Handle(GetAllDoctorsQuery request,
        CancellationToken cancellationToken)
    {
        var doctors = await doctorRepository.GetAllAsync(cancellationToken);
        return Result.Success(DoctorResponse.FromDoctorCollection(doctors));
    }
}