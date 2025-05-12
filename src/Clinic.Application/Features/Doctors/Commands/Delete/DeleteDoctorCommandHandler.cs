using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.Delete;
public sealed class DeleteDoctorCommandHandler(IDoctorRepository doctorRepository)
    : IRequestHandler<DeleteDoctorCommand, Result>
{
    public async Task<Result> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor is null)
        {
            return DoctorErrors.NotFound(request.Id);
        }

        await doctorRepository.DeleteAsync(doctor, cancellationToken);

        return Result.Success();
    }
}