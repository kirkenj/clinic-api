using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class AppointmentResultRepository(ClinnicContext context) : IAppointmentResultRepository
{
    private readonly DbSet<AppointmentResult> _dbSet = context.Set<AppointmentResult>();

    public Task AddAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken)
    {
        context.Add(appointmentResult);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken)
    {
        context.Remove(appointmentResult);
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task<AppointmentResult?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<AppointmentResult>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<AppointmentResult>(collection, page, pageSize, totalCount);
    }

    public async Task UpdateAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken)
    {
        context.Update(appointmentResult);
        await context.SaveChangesAsync(cancellationToken);
    }
}