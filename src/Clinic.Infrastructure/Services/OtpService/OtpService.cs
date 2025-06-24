using Clinic.Application.Abstractions;
using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Errors;
using Clinic.Domain.Repositories;
using Microsoft.Extensions.Options;

namespace Clinic.Infrastructure.Services.OtpService;

public class OtpService(
    IOtpRecordRepository otpRecordRepository,
    IOptions<OtpServiceOptions> options,
    IDateTimeProvider dateTimeProvider)
    : IOtpService
{
    public async Task<Result<string>> GenerateOtpAsync(string otpKey, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(otpKey))
        {
            return Result.Failure<string>(BasicErrors.NullOrEmpty(nameof(otpKey)));
        }

        var oldOtp = await otpRecordRepository.GetRecordAsync(otpKey, cancellationToken);
        if (oldOtp is not null)
        {
            oldOtp.Disable();
            await otpRecordRepository.UpdateAsync(oldOtp, cancellationToken);
        }

        var otp = Random.Shared.Next(options.Value.OtpRangeStart, options.Value.OtpRangeEnd)
            .ToString();
        var otpRecord = OtpRecord.Create(otpKey, otp, 0, options.Value.MaxTriesCount, true,
            dateTimeProvider.UtcNow() + options.Value.OtpTtl);
        await otpRecordRepository.AddAsync(otpRecord, cancellationToken);
        return Result.Success(otp);
    }

    public async Task<Result> CheckOtpAsync(string otpKey, string otp, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(otpKey))
        {
            return BasicErrors.NullOrEmpty(nameof(otpKey));
        }

        if (string.IsNullOrWhiteSpace(otp))
        {
            return BasicErrors.NullOrEmpty(nameof(otp));
        }

        var otpRecord = await otpRecordRepository.GetRecordAsync(otpKey, cancellationToken);
        if (otpRecord == null)
        {
            return OtpRecordErrors.NotFound();
        }

        otpRecord.IncrementTries();
        if (otpRecord.Otp == otp)
        {
            otpRecord.Disable();
            await otpRecordRepository.UpdateAsync(otpRecord, cancellationToken);
            return Result.Success();
        }

        if (otpRecord.MaxTriesCount == otpRecord.TriesCount)
        {
            otpRecord.Disable();
        }

        await otpRecordRepository.UpdateAsync(otpRecord, cancellationToken);
        return OtpRecordErrors.InvalidOtp(otpRecord.MaxTriesCount, otpRecord.TriesCount);
    }
}