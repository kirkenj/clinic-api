using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class Doctor : BaseEntity, IAuditableEntity
{
    private Doctor()
    {
    }

    private Doctor(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, uint specializationId,
        uint officeId)
    {
        IsActive = true;
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
        PhoneNumber = phoneNumber;
        SpecializationId = specializationId;
        OfficeId = officeId;
    }

    public bool IsActive { get; private set; }
    public string FirstName { get; private set; } = default!;
    public string LastName { get; private set; } = default!;
    public string MiddleName { get; private set; } = default!;
    public string PhoneNumber { get; private set; } = default!;
    public string Email { get; private set; } = default!;
    public uint SpecializationId { get; private set; }
    public Specialization Speciallization { get; private set; } = default!;
    public uint OfficeId { get; private set; }
    public Office Office { get; private set; } = default!;
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public void Update(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, uint specializationId,
        uint officeId)
    {
        FirstName = firstName;
        LastName = lastName;
        MiddleName = middleName;
        Email = email;
        PhoneNumber = phoneNumber;
        SpecializationId = specializationId;
        OfficeId = officeId;
    }

    public static Doctor Create(string firstName, string lastName,
        string middleName, string email,
        string phoneNumber, uint specializationId,
        uint officeId)
    {
        return new Doctor(firstName, lastName,
            middleName, email,
            phoneNumber, specializationId,
            officeId);
    }
}