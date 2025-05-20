using Clinic.Application.Extensions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Update;

public sealed class UpdatePatientCommandHandler(
    IPatientRepository patientRepository,
    UpdatePatientCommandValidator validator
) : IRequestHandler<UpdatePatientCommand, Result>
{
    public async Task<Result> Handle(UpdatePatientCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var patient = await patientRepository.GetByIdAsync(request.Id, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.Id);
        }

        if (!patient.IsActive)
        {
            return PatientErrors.NotActive(patient.Id);
        }

        var hasSamePhone = patient.PhoneNumber.HasSameValue(request.PhoneNumber);
        if (!hasSamePhone)
        {
            var isPhoneTaken = await patientRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
            if (isPhoneTaken)
            {
                return PatientErrors.PhoneAlreadyTaken(request.PhoneNumber);
            }
        }

        var hasSameEmail = patient.Email.HasSameValue(request.Email);
        if (!hasSameEmail)
        {
            var isEmailTaken = await patientRepository.ExistsByEmailAsync(request.Email, cancellationToken);
            if (isEmailTaken)
            {
                return PatientErrors.EmailAlreadyTaken(request.Email);
            }
        }

        patient.Update(request.FirstName, request.LastName,
            request.MiddleName, request.Email, request.PhoneNumber);
        await patientRepository.UpdateAsync(patient, cancellationToken);
        return Result.Success();
    }
}