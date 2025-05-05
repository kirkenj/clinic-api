using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Office;

public static class OfficeErrors
{
    public static Error NotFound(uint id) => new("Office.NotFound", $"Office with id {id} was not found");
    public static Error AddressAlreadyTaken(string address) => new("Office.AddressAlreadyTaken", $"Office with address '{address}' already exists");
}