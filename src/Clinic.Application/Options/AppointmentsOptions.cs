namespace Clinic.Application.Options;

public sealed class AppointmentsOptions
{
    public TimeSpan WorkTimeStart { get; init; }
    public TimeSpan WorkTimeEnd { get; init; }
    public DayOfWeek WorkWeekDayStart { get; init; }
    public DayOfWeek WorkWeekDayEnd { get; init; }
    public int TimeSlotSizeMinutes { get; init; }
}