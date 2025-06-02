using Clinic.Application.Options;
using Microsoft.Extensions.Options;

namespace Clinic.Application.Services.SlotService;

public sealed class SlotService : ISlotService
{
    private readonly AppointmentsOptions _options;
    private readonly IReadOnlyCollection<SlotPeriod> _slots;

    public SlotService(IOptions<AppointmentsOptions> options)
    {
        _options = options.Value;
        var dayStart = _options.WorkTimeStart;
        var dayEnd = _options.WorkTimeEnd;

        List<SlotPeriod> slots = [];
        var slotStartDateTime = dayStart;
        var increment = new TimeSpan(0, 0, _options.TimeSlotSizeMinutes, 0);
        var slotEndDateTime = slotStartDateTime + increment;

        while (slotEndDateTime < dayEnd)
        {
            slots.Add(new SlotPeriod(slotStartDateTime, slotEndDateTime));
            slotStartDateTime = slotEndDateTime;
            slotEndDateTime = slotStartDateTime + increment;
        }

        _slots = slots;
    }

    public IReadOnlyCollection<DatedSlot> GetSlots(DateTime dateTime)
    {
        return CheckForValidWorkingDay(dateTime)
            ? _slots.Select(s => s.ToDatedSlot(dateTime)).ToArray()
            : [];
    }

    public DatedSlot? GetSlot(DateTime dateTime)
    {
        if (!CheckForValidWorkingDay(dateTime))
        {
            return null;
        }

        var time = dateTime.TimeOfDay;
        var slot = _slots.FirstOrDefault(s => s.TimeStart <= time && s.TimeEnd >= time);
        return slot?.ToDatedSlot(dateTime);
    }

    private bool CheckForValidWorkingDay(DateTime dateTime)
    {
        var requestDayOfWeek = dateTime.DayOfWeek;
        if (requestDayOfWeek < _options.WorkWeekDayStart ||
            requestDayOfWeek > _options.WorkWeekDayEnd)
        {
            return false;
        }

        var requestTimeOfDay = dateTime.TimeOfDay;
        if (requestTimeOfDay < _options.WorkTimeStart ||
            requestTimeOfDay > _options.WorkTimeStart)
        {
            return false;
        }

        return true;
    }
}