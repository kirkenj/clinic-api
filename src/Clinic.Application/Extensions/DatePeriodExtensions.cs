using Clinic.Domain.Abstractions;

namespace Clinic.Application.Extensions;

public static class DatePeriodExtensions
{
    public static bool IntersectsWith(this (DateTime Start, DateTime End) period1, (DateTime Start, DateTime End) period2)
        => (period1.Start <= period2.End && period2.Start <= period1.End) || 
           (period1.Start >= period2.Start && period1.End <= period2.End) ||
           (period2.Start >= period1.Start && period2.End <= period1.End);
    

    public static bool IntersectsWith(this DatedSlot slot, DateTime periodStart, DateTime periodEnd)
        => (slot.SlotStart, slot.SlotEnd).IntersectsWith((periodStart, periodEnd));
}