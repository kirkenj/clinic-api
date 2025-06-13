namespace Clinic.Application.Tools;

public class EmailBodyBuilder
{
    public static string BuildCancelledAppointmentContent(string serviceName, string patientFirstName,
        string patientLastName, DateTime appointmentStartDateTime)
    {
        return $"Appointment cancelled: '{serviceName}'({patientFirstName} {patientLastName}) " +
               $"at {appointmentStartDateTime}.";
    }

    public static string BuildConfirmAppointmentCancellationContent(string serviceName, string doctorFirstName,
        string doctorLastName, DateTime appointmentStartDateTime, string otp)
    {
        return $"To cancel confirmed appointment ('{serviceName}', {doctorFirstName} {doctorLastName} " +
               $"at {appointmentStartDateTime}) use one-time password '{otp}'.";
    }

    public static string BuildAppointmentRescheduledContent(string serviceName,
        string oldDoctorFirstName, string oldDoctorLastName, DateTime oldAppointmentStartDateTime,
        string newDoctorFirstName, string newDoctorLastName, DateTime newAppointmentStartDateTime)
    {
        return $"Your appointment '{serviceName}' ({oldDoctorFirstName} {oldDoctorLastName}) " +
               $"at {oldAppointmentStartDateTime} was rescheduled for {newDoctorFirstName} " +
               $"({newDoctorLastName} {newAppointmentStartDateTime}).";
    }

    public static string BuildNewAppointmentForDoctorContent(string serviceName,
        string patientFirstName, string patientLastName, DateTime appointmentStartDateTime)
    {
        return $"New appointment: '{serviceName}'({patientFirstName} {patientLastName}) " +
               $"at {appointmentStartDateTime}.";
    }

    public static string BuildConfirmAppointmentContent(string serviceName, string doctorFirstName, string doctorLastName,
        decimal servicePrice, DateTime appointmentStartDateTime, string otp)
    {
        return $"New appointment: '{serviceName}' {servicePrice} ({doctorFirstName} {doctorLastName}) " +
               $"at {appointmentStartDateTime} with one-time password '{otp}'.";
    }

    public static string BuildSignUpContent(string otp)
    {
        return $"To confirm your account use one-time password '{otp}'";
    }

    public static string BuildSignInContent(string otp)
    {
        return $"To sign in use one-time password '{otp}'";
    }
}