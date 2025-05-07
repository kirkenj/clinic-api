using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Office;

public static class OfficeErrors
{
    public static Error NotFound(uint id) => new("Office.NotFound", $"Office with id {id} was not found");
    public static Error PhoneAlreadyTaken(string address) => new("Office.PhoneAlreadyTaken", $"Office with phone number '{address}' already exists");
}