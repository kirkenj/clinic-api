using FluentValidation;

namespace Clinic.Application.Features.Offices.Queries.GetPaginated;
public class GetPaginatedOfficesRequestValidator : AbstractValidator<GetPaginatedOfficesRequest>
{
    public GetPaginatedOfficesRequestValidator()
    {
        RuleFor(r => r.Page).NotEmpty();
        RuleFor(r => r.PageSize).NotEmpty();
    }
}