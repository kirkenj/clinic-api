using Clinic.Application.Responses.Doctor;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Doctor;
using MediatR;

namespace Clinic.Application.Features.Doctor.Queries.Get;

public sealed class GetDoctorQueryHandler(IDoctorRepository doctorRepository) 
    : IRequestHandler<GetDoctorQuery, Result<DoctorResponse>>
{
    public async Task<Result<DoctorResponse>> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
        {
            return Result.Failure<DoctorResponse>(
                DoctorErrors.NotFound(request.Id));
        }

        return DoctorResponse.FromDoctor(doctor);
    }
}