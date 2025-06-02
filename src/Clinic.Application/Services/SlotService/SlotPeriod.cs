namespace Clinic.Application.Services.SlotService;

public record SlotPeriod(TimeSpan TimeStart, TimeSpan TimeEnd)
{
    public DatedSlot ToDatedSlot(DateTime dateTime)
    {
        return new DatedSlot(dateTime.Date + TimeStart, dateTime.Date + TimeEnd);
    }
}