using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class OtpRecordErrors
{
    public static Error NotFound()
    {
        return new Error("OtpRecord.NotFound", "Otp was not found.");
    }

    public static Error InvalidOtp(int maxTriesCount, int triesCount)
    {
        var remainingAttempts = maxTriesCount - triesCount;
        return new Error("InvalidOtp",
            "Invalid otp. " + (remainingAttempts == 0
                ? "No more attempts left. Request a new one-time password"
                : $"Attempts left: {remainingAttempts}."));
    }
}