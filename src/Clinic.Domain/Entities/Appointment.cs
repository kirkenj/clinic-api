using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public class Appointment : BaseEntity, IAuditableEntity
{
    private Appointment()
    {
    }

    private Appointment(uint doctorId, uint patientId,
        uint serviceId, DateTime dateTimeStart, DateTime dateTimeEnd,
        bool isApprovedByUser, bool isApprovedByClinic,
        AppointmentStatus status)
    {
        DoctorId = doctorId;
        PatientId = patientId;
        ServiceId = serviceId;
        DateTimeStart = dateTimeStart;
        DateTimeEnd = dateTimeEnd;
        IsApprovedByClinic = isApprovedByClinic;
        IsApprovedByUser = isApprovedByUser;
        Status = status;
    }

    public uint DoctorId { get; private set; }
    public Doctor Doctor { get; private set; } = default!;
    public uint PatientId { get; private set; }
    public Patient Patient { get; private set; } = default!;
    public uint ServiceId { get; private set; }
    public Service Service { get; private set; } = default!;
    public DateTime DateTimeStart { get; private set; }
    public DateTime DateTimeEnd { get; private set; }
    public bool IsApprovedByClinic { get; private set; }
    public bool IsApprovedByUser { get; private set; }
    public AppointmentStatus Status { get; private set; }
    public AppointmentResult? AppointmentResult { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }

    public void Update(uint doctorId, uint patientId,
        uint serviceId, DateTime dateTimeStart, DateTime dateTimeEnd,
        bool isApprovedByUser, bool isApprovedByClinic,
        AppointmentStatus status)
    {
        if (dateTimeStart >= dateTimeEnd)
        {
            throw new ArgumentException("dateTimeStart is bigger or equal to dateTimeEnd");
        }

        DoctorId = doctorId;
        PatientId = patientId;
        ServiceId = serviceId;
        DateTimeStart = dateTimeStart;
        DateTimeEnd = dateTimeEnd;
        Status = status;
        IsApprovedByClinic = isApprovedByClinic;
        IsApprovedByUser = isApprovedByUser;
    }

    public static Appointment Create(uint doctorId, uint patientId,
        uint serviceId, DateTime dateTimeStart, DateTime dateTimeEnd,
        bool isApprovedByUser, bool isApprovedByClinic,
        AppointmentStatus status)
    {
        if (dateTimeStart >= dateTimeEnd)
        {
            throw new ArgumentException("dateTimeStart is bigger or equal to dateTimeEnd");
        }

        return new Appointment(doctorId, patientId,
            serviceId, dateTimeStart, dateTimeEnd,
            isApprovedByUser, isApprovedByClinic,
            status);
    }
}