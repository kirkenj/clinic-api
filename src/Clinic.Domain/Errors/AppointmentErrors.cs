using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Errors;

public static class AppointmentErrors
{
    public static Error NotFound(uint id)
    {
        return new Error("Appointment.NotFound", $"Appointment with id {id} was not found");
    }

    public static Error DateTimeDoctorTaken(uint doctorId, DateTime dateTime)
    {
        return new Error("Appointment.DateTimeDoctorTaken",
            $"Doctor with id '{doctorId}' is already has got an appointment at {dateTime}");
    }

    public static Error DateTimePatientTaken(uint patientId, DateTime dateTime)
    {
        return new Error("Appointment.DateTimePatientTaken",
            $"Doctor with id '{patientId}' is already has got an appointment at {dateTime}");
    }

    public static Error AppointmentIsApproved(uint appointmentId)
    {
        return new Error("Appointment.AppointmentIsApproved",
            $"Appoint with id '{appointmentId}' is approved");
    }

    public static Error InvalidDayOfWeek(DayOfWeek dayOfWeek, DayOfWeek startDayOfWorkingWeek,
        DayOfWeek endDayOfWorkingWeek)
    {
        return new Error("Appointment.InvalidDayOfWeek",
            $"'{dayOfWeek}' is invalid working day in clinic. It works between '{startDayOfWorkingWeek}' and '{endDayOfWorkingWeek}'");
    }

    public static Error InvalidTimeOfDay(TimeSpan timeOfDay, TimeSpan workingDayTimeStart, TimeSpan workingDayTimeEnd)
    {
        return new Error("Appointment.InvalidDayOfWeek",
            $"'{timeOfDay}' is invalid working time in clinic. It works between '{workingDayTimeStart}' and '{workingDayTimeEnd}'");
    }

    public static Error AppointmentIsNotApprovedByClinic(uint appointmentId)
    {
        return new Error("Appointment.AppointmentIsApproved",
            $"Appoint with id '{appointmentId}' is not approved by clinic");
    }
}