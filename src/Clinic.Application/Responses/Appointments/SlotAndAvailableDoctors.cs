using Clinic.Domain.Abstractions;

namespace Clinic.Application.Responses.Appointments;

public record SlotAndAvailableDoctors(DatedSlot Slot, IEnumerable<uint> AvailableDoctors);