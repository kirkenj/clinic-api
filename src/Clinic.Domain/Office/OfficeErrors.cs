using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Office;

public static class OfficeErrors
{
    public static Error NotFound(uint id) => new("Office.NotFound", $"Office with id {id} was not found");
    public static Error NotActive(uint id) => new("Office.NotActive", $"Office with id {id} is not active");
    public static Error PhoneAlreadyTaken(string phoneNumber) => new("Office.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other office");
}