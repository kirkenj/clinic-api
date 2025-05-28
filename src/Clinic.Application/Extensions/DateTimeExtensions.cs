namespace Clinic.Application.Extensions;

public static class DateTimeExtensions
{
    public static DateTime NormalizeDateTimeToMinutePeriod(this DateTime dateTime, int minutePeriod)
    {
        var minuteDiff = dateTime.Minute % minutePeriod;
        var normalizedToMinutes = dateTime.AddMinutes(-minuteDiff);
        return normalizedToMinutes.AddSeconds(-normalizedToMinutes.Second);
    }
}