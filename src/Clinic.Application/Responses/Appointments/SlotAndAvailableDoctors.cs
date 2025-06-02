using Clinic.Application.Services.SlotService;

namespace Clinic.Application.Responses.Appointments;

public record SlotAndAvailableDoctors(DatedSlot Slot, IEnumerable<uint> AvailableDoctors);