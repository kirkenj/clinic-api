using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Delete;
public sealed class DeletePatientCommandHandler(IPatientRepository patientRepository)
    : IRequestHandler<DeletePatientCommand, Result>
{
    public async Task<Result> Handle(DeletePatientCommand request, CancellationToken cancellationToken)
    {
        var patient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.Id);
        }

        await patientRepository.DeleteAsync(patient, cancellationToken);
        return Result.Success();
    }
}