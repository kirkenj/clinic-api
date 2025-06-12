using Clinic.Application.Abstractions;
using Clinic.Application.Otp;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using MediatR;

namespace Clinic.Application.Features.Doctors.Commands.ValidateSignIn;

public sealed class DoctorValidateSignInRequestHandler(
    DoctorValidateSignInRequestValidator validator,
    IOtpService otpService,
    IDoctorRepository doctorRepository,
    IJwtService jwtService) 
    : IRequestHandler<DoctorValidateSignInRequest, Result<string>>
{
    public async Task<Result<string>> Handle(DoctorValidateSignInRequest request, CancellationToken cancellationToken)
    {
        request.TrimStringProperties();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Result.Failure<string>(Error.ValidationError(
                validationResult.Errors.Select(e => e.ErrorMessage)));
        }

        var otpKey = OtpKeys.DoctorSignInRequest(request.Email);
        var otpGenerationResult = await otpService.ValidateOtpAsync(otpKey, request.Otp, cancellationToken);
        if (otpGenerationResult.IsFailure)
        {
            return Result.Failure<string>(otpGenerationResult.Error);
        }

        var doctor = await doctorRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (doctor is null)
        {
            return Result.Failure<string>(DoctorErrors.NotFound(request.Email));
        }

        if (!doctor.IsActive)
        {
            return Result.Failure<string>(DoctorErrors.NotActive(doctor.Id));
        }

        return Result.Success(jwtService.GenerateToken(doctor.Id, RoleType.Doctor));
    }
}