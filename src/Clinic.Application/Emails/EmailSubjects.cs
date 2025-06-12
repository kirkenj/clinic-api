namespace Clinic.Application.Emails;

public static class EmailSubjects
{
    public static string ConfirmAppointment => "Appointment creation";

    public static string AppointmentCancelled => "Appointment cancelled";

    public static string AppointmentRescheduled => "Appointment rescheduled";
    
    public static string ConfirmAppointmentCancellationByUser => "Confirm appointment cancellation";

    public static string NewAppointment => "New appointment";
}