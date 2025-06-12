using Clinic.Application.Abstractions;
using Clinic.Application.Otp;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ValidateSignUp;

public sealed class PatientValidateSignUpRequestHandler(
    PatientValidateSignUpRequestValidator validator,
    IOtpService otpService,
    IPatientRepository patientRepository)
    : IRequestHandler<ValidateSignIn.PatientValidateSignUpRequest, Result>
{
    public async Task<Result> Handle(ValidateSignIn.PatientValidateSignUpRequest request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var otpKey = OtpKeys.AddPatientCommand(request.Email);
        var otpGenerationResult = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return otpGenerationResult.Error;
        }

        var patient = await patientRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (patient is null)
        {
            return PatientErrors.NotFound(request.Email);
        }

        if (patient.IsActive)
        {
            return PatientErrors.Active(patient.Id);
        }

        patient.Update(patient.FirstName, patient.LastName, patient.MiddleName, 
            patient.Email, patient.PhoneNumber, true);
        await patientRepository.UpdateAsync(patient, cancellationToken);
        return Result.Success();
    }
}