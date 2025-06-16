namespace Clinic.Domain.Abstractions;

public interface IDateTimeProvider
{
    public DateTime UtcNow();
}