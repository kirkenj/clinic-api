using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
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

    public Task<bool> ExistsByAppointmentIdAsync(uint appointmentId, CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(r => r.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<PaginatedCollection<AppointmentResult>> GetPaginatedAsync(AppointmentResultFilter? appointmentResultFilter, 
        int page, int pageSize,CancellationToken cancellationToken)
    {
        var set = _dbSet.AsQueryable();
        if (appointmentResultFilter is not null)
        {
            if (appointmentResultFilter.Ids is not null && appointmentResultFilter.Ids.Count != 0)
            {
                set = set.Where(o => appointmentResultFilter.Ids.Contains(o.Id));
            }

            set = appointmentResultFilter.OrderDescending
                ? set.OrderByDescending(a => a.Id)
                : set.OrderBy(a => a.Id);
        }
        else
        {
            set = set.OrderBy(s => s.Id);
        }

        var totalCount = await set.CountAsync(cancellationToken);

        var collection = await set.AsNoTracking()
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