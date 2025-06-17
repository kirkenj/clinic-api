using Clinic.Domain.Abstractions;

namespace Clinic.Infrastructure.Services.DateTimeProvider;

public sealed class DateTimeProvider : IDateTimeProvider
{
    public DateTime UtcNow()
    {
        return DateTime.UtcNow;
    }
}