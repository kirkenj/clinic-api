using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.Delete;
public sealed class DeleteOfficeCommandHandler(IOfficeRepository officeRepository) : IRequestHandler<DeleteOfficeCommand, Result>
{
    public async Task<Result> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var office = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (office == null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        await officeRepository.DeleteAsync(office, cancellationToken);

        return Result.Success();
    }
}