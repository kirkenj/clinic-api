using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class OtpRecord : IAuditableEntity
{
    private OtpRecord()
    {
    }

    private OtpRecord(string key, string otpHash, int triesCount,
        int maxTriesCount, bool isActive, DateTime expiresOnUtc)
    {
        Key = key;
        OtpHash = otpHash;
        TriesCount = triesCount;
        MaxTriesCount = maxTriesCount;
        IsActive = isActive;
        ExpiresOnUtc = expiresOnUtc;
    }

    public string Key { get; private set; } = default!;
    public string OtpHash { get; private set; } = default!;
    public int TriesCount { get; private set; }
    public int MaxTriesCount { get; private set; }
    public bool IsActive { get; private set; }
    public DateTimeOffset ExpiresOnUtc { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public static OtpRecord Create(string key, string otpHash, int triesCount,
        int maxTriesCount, bool isActive, DateTime expiresOnUtc)
    {
        return new OtpRecord(key, otpHash, triesCount, maxTriesCount, isActive, expiresOnUtc);
    }

    public void IncrementTries()
    {
        TriesCount++;
    }

    public void Disable()
    {
        IsActive = false;
    }
}