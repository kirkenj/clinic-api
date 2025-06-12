namespace Clinic.Application.Otp;

public static class OtpKeys
{
    public static string ConfirmAppointmentCancellationByUser(uint patientId, uint appointmentId)
    {
        return $"ConfirmAppointmentCancellationByPatient{patientId}:{appointmentId}";
    }

    public static string ConfirmAppointmentCreationByPatient(uint patientId, uint appointmentId)
    {
        return $"ConfirmAppointmentCreationByPatient{patientId}:{appointmentId}";
    }
}