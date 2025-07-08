using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class AppointmentErrors
{
    public static Error InvalidDate(DateTime date)
    {
        return new Error("Appointment.InvalidDate", $"Date {date} is not valid");
    }

    public static Error NotFound(uint id)
    {
        return new Error("Appointment.NotFound", $"Appointment with id {id} was not found");
    }

    public static Error NotExpectedPatientId()
    {
        return new Error("Appointment.NotExpectedPatientId", $"Not expected patient id");
    }

    public static Error DateTimeDoctorTaken(uint doctorId, DateTime dateTime)
    {
        return new Error("Appointment.DateTimeDoctorTaken",
            $"Doctor with id '{doctorId}' has already got an appointment at {dateTime}");
    }

    public static Error AlreadyExists(uint appointmentId)
    {
        return new Error("Appointment.AlreadyExists",
            $"Appointment already exists with id '{appointmentId}'");
    }

    public static Error DateTimePatientTaken(uint patientId, DateTime dateTime)
    {
        return new Error("Appointment.DateTimePatientTaken",
            $"Patient with id '{patientId}' has already got an appointment at {dateTime}");
    }

    public static Error AppointmentIsApproved(uint appointmentId)
    {
        return new Error("Appointment.AppointmentIsApprovedByClinic",
            $"Appointment with id '{appointmentId}' is approved");
    }

    public static Error AppointmentIsNotApprovedByUser(uint appointmentId)
    {
        return new Error("Appointment.AppointmentIsNotApprovedByUser",
            $"Appointment with id '{appointmentId}' is not approved by user");
    }

    public static Error NoSlotsFound(DateTime dateTime)
    {
        return new Error("Appointment.InvalidDayOfWeek",
            $"No slots found at '{dateTime}'");
    }

    public static Error InvalidStatus(AppointmentStatus currentStatus, params AppointmentStatus[] expectedStatuses)
    {
        return new Error("Appointment.InvalidStatus",
            $"Unexpected appointment status '{currentStatus}'. Expected: one of [{string.Join(", ", expectedStatuses.Select(s => $"'{s}'"))}]");
    }

    public static Error HasNotTakenPlace(uint appointmentId)
    {
        return new Error("Appointment.HasNotTakenPlace",
            $"Appointment with id {appointmentId}' hasn't taken place.");
    }
}