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

    public static string DoctorSignInRequest(string email)
    {
        return $"DoctorSignInRequest:{email}";
    }

    public static string PatientSignInRequest(string email)
    {
        return $"PatientSignInRequest:{email}";
    }

    public static string ReceptionistSignInRequest(string email)
    {
        return $"ReceptionistSignInRequest:{email}";
    }

    public static string AddPatientCommand(string email)
    {
        return $"AddPatientCommand:{email}";
    }
}