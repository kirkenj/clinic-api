using Clinic.Domain.Abstractions;

namespace Clinic.Domain.Entities;

public sealed class AppointmentResult : BaseEntity, IAuditableEntity
{
    private AppointmentResult()
    {
    }

    private AppointmentResult(string complaints, string conclusion,
        string recommendations, uint appointmentId)
    {
        Complaints = complaints;
        Conclusion = conclusion;
        Recommendations = recommendations;
        AppointmentId = appointmentId;
    }

    public string Complaints { get; private set; } = default!;
    public string Conclusion { get; private set; } = default!;
    public string Recommendations { get; private set; } = default!;
    public Appointment Appointment { get; private set; } = default!;
    public uint AppointmentId { get; private set; }
    public DateTimeOffset CreatedOnUtc { get; set; }
    public DateTimeOffset? UpdatedOnUtc { get; set; }


    public static AppointmentResult Create(string complaints, string conclusion,
        string recommendations, uint appointmentId)
    {
        return new AppointmentResult(complaints, conclusion,
            recommendations, appointmentId);
    }

    public void Update(string complaints, string conclusion,
        string recommendations, uint appointmentId)
    {
        Complaints = complaints;
        Conclusion = conclusion;
        Recommendations = recommendations;
        AppointmentId = appointmentId;
    }
}