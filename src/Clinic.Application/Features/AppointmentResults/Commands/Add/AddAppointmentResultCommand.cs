using Clinic.Domain.Abstractions;
using MediatR;

namespace Clinic.Application.Features.AppointmentResults.Commands.Add;

public sealed class AddAppointmentResultCommand : IRequest<Result>
{
    public string Complaints { get; set; } = default!;
    public string Conclusion { get; set; } = default!;
    public string Recommendations { get; set; } = default!;
    public uint AppointmentId { get; set; }


    public void TrimStringProperties()
    {
        Complaints = Complaints.Trim();
        Conclusion = Conclusion.Trim();
        Recommendations = Recommendations.Trim();
    }
}