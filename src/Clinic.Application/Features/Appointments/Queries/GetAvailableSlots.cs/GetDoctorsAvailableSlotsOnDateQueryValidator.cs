using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetAvailableSlots.cs;
public class GetDoctorsAvailableSlotsOnDateQueryValidator : AbstractValidator<GetDoctorsAvailableSlotsOnDateQuery>
{
    public GetDoctorsAvailableSlotsOnDateQueryValidator()
    {
        RuleFor(r => r.Date)
            .NotEmpty()
            .GreaterThanOrEqualTo(DateTime.UtcNow.Date);
        RuleFor(r => r.DoctorId).NotEmpty();
    }
}
