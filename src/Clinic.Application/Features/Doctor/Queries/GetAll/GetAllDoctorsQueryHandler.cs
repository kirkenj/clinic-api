using Clinic.Application.Responses.Doctor;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Doctor;
using MediatR;

namespace Clinic.Application.Features.Doctor.Queries.GetAll;
public sealed class GetAllDoctorsQueryHandler(IDoctorRepository doctorRepository) 
    : IRequestHandler<GetAllDoctorsQuery, Result<IEnumerable<DoctorResponse>>>
{
    public async Task<Result<IEnumerable<DoctorResponse>>> Handle(GetAllDoctorsQuery request, CancellationToken cancellationToken)
    {
        var doctors = await doctorRepository.GetAllAsync(cancellationToken, false);
        return Result.Success(DoctorResponse.FromDoctorCollection(doctors));
    }
}
