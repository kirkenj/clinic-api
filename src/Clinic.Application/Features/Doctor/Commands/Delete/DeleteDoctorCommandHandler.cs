using Clinic.Domain.Abstractions;
using Clinic.Domain.Doctor;
using MediatR;

namespace Clinic.Application.Features.Doctor.Commands.Delete;
public sealed class DeleteDoctorCommandHandler(IDoctorRepository doctorRepository) 
    : IRequestHandler<DeleteDoctorCommand, Result>
{
    public async Task<Result> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await doctorRepository.GetByIdAsync(request.Id, cancellationToken);
        if (doctor == null)
        {
            return DoctorErrors.NotFound(request.Id);
        }

        await doctorRepository.DeleteAsync(doctor, cancellationToken);

        return Result.Success();
    }
}