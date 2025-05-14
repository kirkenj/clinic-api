using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Add;
public sealed class AddPatientCommandHandler(
    IPatientRepository patientRepository,
    AddPatientCommandValidator validator
    ) : IRequestHandler<AddPatientCommand, Result>
{
    public async Task<Result> Handle(AddPatientCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var isPhoneTaken = await patientRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (isPhoneTaken)
        {
            return PatientErrors.PhoneAlreadyTaken(request.PhoneNumber);
        }

        var isEmailTaken = await patientRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (isEmailTaken)
        {
            return PatientErrors.EmailAlreadyTaken(request.Email);
        }

        var patient = Patient.Create(request.FirstName, request.LastName,
            request.MiddleName, request.Email, request.PhoneNumber);

        await patientRepository.AddAsync(patient, cancellationToken);

        return Result.Success();
    }
}