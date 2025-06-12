using FluentValidation;

namespace Clinic.Application.Features.Appointments.Queries.GetForDoctorOnDate;

public sealed class GetRequestsForDoctorOnDateRequestValidator : AbstractValidator<GetRequestsForDoctorOnDateRequest>
{
    public GetRequestsForDoctorOnDateRequestValidator()
    {
        RuleFor(r => r.DoctorId)
            .NotEmpty();
        RuleFor(r => r.Date)
            .NotEmpty();
    }
}