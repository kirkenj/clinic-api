namespace Clinic.Application.Options;

public sealed class AppointmentsOptions
{
    public TimeSpan WorkTimeStart { get; set; }
    public TimeSpan WorkTimeEnd { get; set; }
    public DayOfWeek WorkWeekDayStart { get; set; }
    public DayOfWeek WorkWeekDayEnd { get; set; }
    public int TimeSlotSizeMinutes { get; set; }
}