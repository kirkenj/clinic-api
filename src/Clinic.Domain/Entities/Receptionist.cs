using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Receptionist : BaseEntity, IAuditableEntity
{
    private Receptionist()
    {
    }

    private Receptionist(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber)
    {
        IsActive = true;
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
    }

    public string FirstName { get; private set; } = default!;

    public string LastName { get; private set; } = default!;

    public string MiddleName { get; private set; } = default!;

    public bool IsActive { get; private set; }

    public string PhoneNumber { get; private set; } = default!;

    public string Email { get; private set; } = default!;

    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public void Update(string firstName, string lastName,
        string middleName, string phoneNumber,
        string email)
    {
        PhoneNumber = phoneNumber;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
    }

    public static Receptionist Create(string firstName, string lastName,
        string middleName, string phoneNumber,
        string email)
    {
        return new Receptionist(firstName, lastName,
            middleName, email,
            phoneNumber);
    }
}