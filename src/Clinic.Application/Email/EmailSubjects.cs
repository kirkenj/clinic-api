namespace Clinic.Application.Email;

public static class EmailSubjects
{
    public static string ConfirmAppointment => "Appointment creation";

    public static string AppointmentCancelled => "Appointment cancelled";

    public static string AppointmentRescheduled => "Appointment rescheduled";
    
    public static string ConfirmAppointmentCancellationByUser => "Confirm appointment cancellation";

    public static string NewAppointment => "New appointment";

    public static string SignIn => "Sign in";
}