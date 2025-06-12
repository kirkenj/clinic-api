using Clinic.Application.Abstractions;
using Clinic.Application.Email;
using Clinic.Application.Otp;
using Clinic.Application.Responses;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignIn;

public class PatientSignInRequestHandler(
    PatientSignInRequestValidator validator,
    IOtpService otpService,
    IPatientRepository patientRepository,
    IEmailService emailService) : IRequestHandler<PatientSignInRequest, Result<string>>
{
    public async Task<Result<string>> Handle(PatientSignInRequest request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
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

        var otpKey = OtpKeys.PatientSignInRequest(patient.Email);
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(patient.Email, EmailSubjects.SignIn, 
            EmailBodies.SignIn(otpGenerationResult.Value), cancellationToken);

        return Result.Success(StringResponses.CheckEmailForFurtherDetails);
    }
}