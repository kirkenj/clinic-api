using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.ConfirmSignIn;

public sealed class ConfirmPatientSignInOtpCommandHandler(
    ConfirmPatientSignInOtpCommandValidator validator,
    IOtpService otpService,
    IPatientRepository patientRepository,
    IJwtService jwtService)
    : IRequestHandler<ConfirmPatientSignInOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(ConfirmPatientSignInOtpCommand request,
        CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var otpKey = $"PatientSignInRequest:{request.Email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = await patientRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (patient is null)
        {
            return Result.Failure<string>(PatientErrors.NotFound(request.Email));
        }

        if (!patient.IsActive)
        {
            return Result.Failure<string>(PatientErrors.NotActive(patient.Id));
        }

        return Result.Success(jwtService.GenerateToken(patient.Id, RoleType.Patient));
    }
}