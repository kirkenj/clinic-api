namespace Clinic.Application.Emails;

public static class EmailBodies
{
    public static string AppointmentCancelledForDoctor(string serviceName, string patientFirstName,
        string patientLastName, DateTime appointmentStartDateTime)
    {
        return
            $"Appointment cancelled: '{serviceName}'({patientFirstName} {patientLastName}) at {appointmentStartDateTime}.";
    }

    public static string AppointmentCancelledForPatient(string serviceName, string doctorFirstName,
        string doctorLastName, DateTime appointmentStartDateTime)
    {
        return
            $"Appointment cancelled: '{serviceName}'({doctorFirstName} {doctorLastName}) at {appointmentStartDateTime}.";
    }

    public static string ConfirmAppointmentCancellation(string serviceName, string doctorFirstName,
        string doctorLastName, DateTime appointmentStartDateTime, string otp)
    {
        return
            $"To cancel confirmed appointment ('{serviceName}', {doctorFirstName} {doctorLastName} " +
            $"at {appointmentStartDateTime}) use one-time password '{otp}'.";
    }

    public static string AppointmentRescheduled(string serviceName,
        string oldDoctorFirstName, string oldDoctorLastName, DateTime oldAppointmentStartDateTime,
        string newDoctorFirstName, string newDoctorLastName, DateTime newAppointmentStartDateTime)
    {
        return
            $"Your appointment '{serviceName}' ({oldDoctorFirstName} {oldDoctorLastName}) at {oldAppointmentStartDateTime} " +
            $"was rescheduled for {newDoctorFirstName} ({newDoctorLastName} {newAppointmentStartDateTime}).";
    }

    public static string NewAppointmentForDoctor(string serviceName,
        string patientFirstName, string patientLastName, DateTime appointmentStartDateTime)
    {
        return $"New appointment: '{serviceName}'({patientFirstName} {patientLastName}) at {appointmentStartDateTime}.";
    }

    public static string ConfirmAppointment(string serviceName, string doctorFirstName, string doctorLastName, decimal servicePrice,
        DateTime appointmentStartDateTime, string otp)
    {
        return $"New appointment: '{serviceName}' {servicePrice} ({doctorFirstName} {doctorLastName}) at {appointmentStartDateTime} "
            + $"with one-time password '{otp}'.";
    }
}