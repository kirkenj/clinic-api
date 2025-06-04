using Clinic.Application.Abstractions;
using Clinic.Application.Options;
using Clinic.Domain.Abstractions;
using Microsoft.Extensions.Options;

namespace Clinic.Infrastructure.Services.SlotService;

public sealed class SlotService(IOptions<AppointmentsOptions> options) : ISlotService
{
    public IReadOnlyCollection<DatedSlot> GetSlots(DateTime dateTime)
    {
        if (!CheckForValidWorkingDay(dateTime))
        {
            return [];
        }

        List<DatedSlot> slots = [];
        var slotStartTime = options.Value.WorkTimeStart;
        var increment = TimeSpan.FromMinutes(options.Value.TimeSlotSizeMinutes);
        while (slotStartTime < options.Value.WorkTimeEnd)
        {
            slots.Add(new DatedSlot(
                dateTime.Date + slotStartTime, 
                dateTime.Date + slotStartTime + increment));
            slotStartTime += increment;
        }

        return slots;
    }

    public DatedSlot? GetSlot(DateTime dateTime)
    {
        if (!CheckForValidWorkingDay(dateTime))
        {
            return null;
        }

        var minuteOfWorkingDay = (int)(dateTime.TimeOfDay - options.Value.WorkTimeStart)
            .TotalMinutes;
        var slotNumber = minuteOfWorkingDay / options.Value.TimeSlotSizeMinutes;
        var periodStart = options.Value.WorkTimeStart +
                          TimeSpan.FromMinutes(slotNumber * options.Value.TimeSlotSizeMinutes);
        return new DatedSlot(
            dateTime.Date + periodStart,
            dateTime.Date + periodStart + 
                TimeSpan.FromMinutes(options.Value.TimeSlotSizeMinutes));
    }

    private bool CheckForValidWorkingDay(DateTime dateTime)
    {
        return dateTime.DayOfWeek >= options.Value.WorkWeekDayStart &&
               dateTime.DayOfWeek <= options.Value.WorkWeekDayEnd &&
               dateTime.TimeOfDay >= options.Value.WorkTimeStart &&
               dateTime.TimeOfDay <= options.Value.WorkTimeEnd;
    }
}