namespace Clinic.Application.Services.SlotService;

public interface ISlotService
{
    public IReadOnlyCollection<DatedSlot> GetSlots(DateTime dateTime);
    public DatedSlot? GetSlot(DateTime dateTime);
}