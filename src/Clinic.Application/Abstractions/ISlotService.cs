using Clinic.Domain.Abstractions;

namespace Clinic.Application.Abstractions;

public interface ISlotService
{
    public IReadOnlyCollection<DatedSlot> GetSlots(DateTime dateTime);
    public DatedSlot? GetSlot(DateTime dateTime);
}