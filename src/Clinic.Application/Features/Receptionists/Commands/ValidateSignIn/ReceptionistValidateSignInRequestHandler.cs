using Clinic.Application.Abstractions;
using Clinic.Application.Otp;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Receptionists.Commands.ValidateSignIn;

public sealed class ReceptionistValidateSignInRequestHandler(
    ReceptionistValidateSignInRequestValidator validator,
    IOtpService otpService,
    IReceptionistRepository receptionistRepository,
    IJwtService jwtService) 
    : IRequestHandler<ReceptionistValidateSignInRequest, Result<string>>
{
    public async Task<Result<string>> Handle(ReceptionistValidateSignInRequest request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var otpKey = OtpKeys.ReceptionistSignInRequest(request.Email);
        var otpGenerationResult = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var receptionist = await receptionistRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (receptionist is null)
        {
            return Result.Failure<string>(ReceptionistErrors.NotFound(request.Email));
        }

        if (!receptionist.IsActive)
        {
            return Result.Failure<string>(ReceptionistErrors.NotActive(receptionist.Id));
        }

        return Result.Success(jwtService.GenerateToken(receptionist.Id, RoleType.Receptionist));
    }
}