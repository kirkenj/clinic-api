using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public class Appointment : BaseEntity, IAuditableEntity
{
    private Appointment()
    {
    }

    private Appointment(uint doctorId, uint patientId, 
        uint serviceId, DateTime dateTime)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        ServiceId = serviceId;
        DateTime = dateTime;
    }

    public uint DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;
    public uint PatientId { get; private set; }
    public Patient Patient { get; private set; } = default!;
    public uint ServiceId { get; private set; }
    public Service Service { get; private set; } = default!;
    public DateTime DateTime { get; private set; }
    public bool IsApproved { get; private set; }
    public bool IsBooked { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(uint doctorId, uint patientId, 
        uint serviceId, DateTime dateTime)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        ServiceId = serviceId;
        DateTime = dateTime;
    }

    public void SetBooked(bool isBooked = true)
    {
        IsBooked = isBooked;
    }

    public void SetApproved(bool isApproved = true)
    {
        IsApproved = isApproved;
    }

    public static Appointment Create(uint doctorId, uint patientId,
        uint serviceId, DateTime dateTime)
    {
        return new Appointment(doctorId, patientId,
            serviceId, dateTime);
    }
}