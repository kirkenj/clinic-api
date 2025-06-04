namespace Clinic.Infrastructure.Services.OtpService;

public sealed class OtpServiceOptions
{
    public int MaxTriesCount { get; init; }
    public TimeSpan OtpTtl { get; init; }
    public int OtpRangeStart { get; init; }
    public int OtpRangeEnd { get; init; }
}