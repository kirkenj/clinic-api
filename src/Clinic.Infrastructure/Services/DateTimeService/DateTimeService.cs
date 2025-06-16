using Clinic.Domain.Abstractions;

namespace Clinic.Infrastructure.Services.DateTimeService;

public sealed class DateTimeService : IDateTimeService
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}