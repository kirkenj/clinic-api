using Clinic.Application.Abstractions;
using Clinic.Application.Responses.Receptionists;
using Clinic.Application.Tools;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;

namespace Clinic.Application.Services;
public class ReceptionistService
    (IReceptionistRepository receptionistRepository,
    IOtpService otpService,
    IJwtService jwtService,
    IEmailService emailService)
    : IReceptionistService
{
    public async Task<Result<ReceptionistResponse>> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        var receptionist = await receptionistRepository.GetByIdAsync(id, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<ReceptionistResponse>(
                ReceptionistErrors.NotFound(id));
        }

        return ReceptionistResponse.FromReceptionist(receptionist);
    }

    public async Task<Result<string>> SendSignInOtpAsync(string email, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        var receptionist = await receptionistRepository.GetByEmailAsync(email, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<string>(ReceptionistErrors.NotFound(email));
        }

        if (!receptionist.IsActive)
        {
            return Result.Failure<string>(ReceptionistErrors.NotActive(receptionist.Id));
        }

        var otpKey = $"ReceptionistSignInRequest:{receptionist.Email}";
        var otpGenerationResult = await otpService.GenerateOtpAsync(otpKey, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        await emailService.SendAsync(receptionist.Email, Constants.SubjectSignIn,
            EmailBodyBuilder.BuildSignInContent(otpGenerationResult.Value), cancellationToken);

        return Result.Success(Constants.ResponseCheckEmailForFurtherDetails);
    }

    public async Task<Result<string>> ValidateSignInAsync(string email, string otp, CancellationToken cancellationToken)
    {
        email = email.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(email);
        otp = otp.Trim();
        ArgumentNullException.ThrowIfNullOrWhiteSpace(otp);
        var otpKey = $"ReceptionistSignInRequest:{email}";
        var otpGenerationResult = await otpService.CheckOtpAsync(otpKey, otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var receptionist = await receptionistRepository.GetByEmailAsync(email, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<string>(ReceptionistErrors.NotFound(email));
        }

        if (!receptionist.IsActive)
        {
            return Result.Failure<string>(ReceptionistErrors.NotActive(receptionist.Id));
        }

        return Result.Success(jwtService.GenerateToken(receptionist.Id, RoleType.Receptionist));

    }
}
