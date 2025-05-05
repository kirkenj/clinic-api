using Clinic.Domain.Abstractions;
using Clinic.Domain.Office;
using MediatR;

namespace Clinic.Application.Features.Office.Commands.DeleteOfficeCommand;
public sealed class DeleteOfficeCommandHandler(IOfficeRepository officeRepository) : IRequestHandler<DeleteOfficeCommand, Result>
{
    public async Task<Result> Handle(DeleteOfficeCommand request, CancellationToken cancellationToken)
    {
        var officeToRemove = await officeRepository.GetByIdAsync(request.Id, cancellationToken);
        if (officeToRemove == null)
        {
            return OfficeErrors.NotFound(request.Id);
        }

        await officeRepository.DeleteAsync(officeToRemove, cancellationToken);
        
        return Result.Success();
    }
}