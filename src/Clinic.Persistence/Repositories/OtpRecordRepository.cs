using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public class OtpRecordRepository(ClinnicContext context) : IOtpRecordRepository
{
    private readonly IQueryable<OtpRecord> _dbSet = context.Set<OtpRecord>();

    public Task<OtpRecord?> GetRecordAsync(string key, CancellationToken cancellationToken)
    {
        return _dbSet
            .OrderByDescending(o => o.CreatedOnUtc)
            .FirstOrDefaultAsync(o =>
                    o.IsActive &&
                    o.MaxTriesCount > o.TriesCount &&
                    o.Key == key &&
                    o.ExpiresOnUtc > DateTime.UtcNow,
                cancellationToken);
    }

    public Task AddAsync(OtpRecord otpRecord, CancellationToken cancellationToken)
    {
        context.Add(otpRecord);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(OtpRecord otpRecord, CancellationToken cancellationToken)
    {
        context.Update(otpRecord);
        return context.SaveChangesAsync(cancellationToken);
    }
}