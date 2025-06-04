using Clinic.Domain.Entities;

namespace Clinic.Domain.Repositories;

public interface IOtpRecordRepository
{
    public Task<OtpRecord?> GetRecordAsync(string key, CancellationToken cancellationToken);
    public Task AddAsync(OtpRecord otpRecord, CancellationToken cancellationToken);
    public Task UpdateAsync(OtpRecord otpRecord, CancellationToken cancellationToken);
}