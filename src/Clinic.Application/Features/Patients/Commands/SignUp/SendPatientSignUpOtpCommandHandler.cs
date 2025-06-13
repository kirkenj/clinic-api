using Clinic.Application.Abstractions;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Patients.Commands.SignUp;

public sealed class SendPatientSignUpOtpCommandHandler(
    IPatientRepository patientRepository,
    IOtpService otpService,
    SendPatientSignUpOtpCommandValidator validator,
    IEmailService emailService)
    : IRequestHandler<SendPatientSignUpOtpCommand, Result<string>>
{
    public async Task<Result<string>> Handle(SendPatientSignUpOtpCommand request, CancellationToken cancellationToken)
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

        var otpKey = $"AddPatientCommand:{request.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var patient = Patient.Create(request.FirstName, request.LastName,
            request.MiddleName, request.Email, request.PhoneNumber);
        patient.Deactivate();
        await patientRepository.AddAsync(patient, cancellationToken);
        await emailService.SendAsync(patient.Email, Constants.SubjectSignUp,
            EmailBodyBuilder.BuildSignUpContent(otpGenerationResult.Value), cancellationToken);
        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }
}