using Clinic.Application.Responses.Doctors;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Queries.Get;

public sealed class GetDoctorQueryHandler(IDoctorRepository doctorRepository)
    : IRequestHandler<GetDoctorQuery, Result<DoctorResponse>>
{
    public async Task<Result<DoctorResponse>> Handle(GetDoctorQuery request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<DoctorResponse>(
                DoctorErrors.NotFound(request.Id));
        }

        return DoctorResponse.FromDoctor(doctor);
    }
}