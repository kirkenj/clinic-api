using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public class Patient : BaseEntity, IAuditableEntity
{
    private Patient()
    {
    }

    private Patient(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, bool isActive)
    {
        IsActive = isActive;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
        PhoneNumber = phoneNumber;
    }

    public bool IsActive { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string MiddleName { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public ICollection<Appointment> Appointments { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, bool isActive)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
        PhoneNumber = phoneNumber;
        IsActive = isActive;
    }

    public static Patient Create(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, bool isActive)
    {
        return new Patient(firstName, lastName,
            middleName, email,
            phoneNumber, isActive);
    }
}