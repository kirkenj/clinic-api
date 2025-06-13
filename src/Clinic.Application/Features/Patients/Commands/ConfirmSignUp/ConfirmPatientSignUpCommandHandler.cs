using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignUp;

public sealed class ConfirmPatientSignUpCommandHandler(
    ConfirmPatientSignUpCommandValidator validator,
    IOtpService otpService,
    IPatientRepository patientRepository)
    : IRequestHandler<ConfirmPatientSignUpCommand, Result>
{
    public async Task<Result> Handle(ConfirmPatientSignUpCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage));
        }

        var otpKey = $"AddPatientCommand:{request.Email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, request.Otp, cancellationToken);
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

        patient.Activate();
        await patientRepository.UpdateAsync(patient, cancellationToken);
        return Result.Success();
    }
}