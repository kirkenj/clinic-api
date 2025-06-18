using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class OfficeErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Office.NotFound", $"Office with id {id} was not found");
    }

    public static Error NotActive(uint id)
    {
        return new Error("Office.NotActive", $"Office with id {id} is not active");
    }

    public static Error PhoneAlreadyTaken(string phoneNumber)
    {
        return new Error("Office.PhoneAlreadyTaken", $"Phone number '{phoneNumber}' is already taken by other office");
    }

    public static Result AddressAlreadyTaken(string address)
    {
        return new Error("Office.AddressAlreadyTaken", $"Address '{address}' is already taken by other office");
    }
}