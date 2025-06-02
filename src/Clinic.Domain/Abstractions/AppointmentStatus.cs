namespace Clinic.Domain.Abstractions;

public enum AppointmentStatus
{
    WaitingUserConfirm,
    WaitingClinicConfirm,
    Rescheduled,
    Confirmed,
    Completed,
    CancelledByUser,
    CancelledByClinic
}