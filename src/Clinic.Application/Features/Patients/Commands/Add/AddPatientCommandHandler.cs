using Clinic.Application.Abstractions;
using Clinic.Application.Otp;
using Clinic.Application.Responses;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.Add;

public sealed class AddPatientCommandHandler
    (IPatientRepository patientRepository,
        IOtpService otpService,
        AddPatientCommandValidator validator)
    : IRequestHandler<AddPatientCommand, Result<string>>
{
    public async Task<Result<string>> Handle(AddPatientCommand request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var isPhoneTaken = await patientRepository.ExistsByPhoneNumberAsync(request.PhoneNumber, cancellationToken);
        if (isPhoneTaken)
        {
            return Result.Failure<string>(PatientErrors.PhoneAlreadyTaken(request.PhoneNumber));
        }

        var isEmailTaken = await patientRepository.ExistsByEmailAsync(request.Email, cancellationToken);
        if (isEmailTaken)
        {
            return Result.Failure<string>(PatientErrors.EmailAlreadyTaken(request.Email));
        }

        var otpKey = OtpKeys.AddPatientCommand(request.Email);
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = Patient.Create(request.FirstName, request.LastName,
            request.MiddleName, request.Email, request.PhoneNumber, false);
        await patientRepository.AddAsync(patient, cancellationToken);
        return Result.Success(StringResponses.CheckEmailForFurtherDetails);
    }
}