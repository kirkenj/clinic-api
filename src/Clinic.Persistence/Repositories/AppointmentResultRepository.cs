using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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
        return await FilterAsync(set, appointmentResultFilter, page, pageSize, cancellationToken);
    }

    private async Task<PaginatedCollection<AppointmentResult>> FilterAsync(IQueryable<AppointmentResult> set, AppointmentResultFilter? appointmentResultFilter, int page, int pageSize, CancellationToken cancellationToken)
    {
        if (appointmentResultFilter is not null)
        {
            if (appointmentResultFilter.Ids is not null && appointmentResultFilter.Ids.Count != 0)
            {
                set = set.Where(o => appointmentResultFilter.Ids.Contains(o.Id));
            }

            if (appointmentResultFilter.AppointmentIds is not null && appointmentResultFilter.AppointmentIds.Count != 0)
            {
                set = set.Where(o => appointmentResultFilter.AppointmentIds.Contains(o.AppointmentId));
            }

            set = appointmentResultFilter.OrderDescending
                ? set.OrderByDescending(GetSortProperty(appointmentResultFilter))
                : set.OrderBy(GetSortProperty(appointmentResultFilter));
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

    private static Expression<Func<AppointmentResult, object>> GetSortProperty(AppointmentResultFilter appointmentResultFilter)
    {
        return appointmentResultFilter.SortColumn switch
        {
            AppointmentResultSortColumn.Id => service => service.Id,
            AppointmentResultSortColumn.AppointmentId => service => service.AppointmentId,
            _ => service => service.Id
        };
    }

    public async Task UpdateAsync(AppointmentResult appointmentResult, CancellationToken cancellationToken)
    {
        context.Update(appointmentResult);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<AppointmentResult?> GetByAppointmentIdAsync(uint appointmentId, CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(r => r.AppointmentId == appointmentId, cancellationToken);
    }

    public async Task<PaginatedCollection<AppointmentResult>> GetPaginatedAsync(uint patientId, AppointmentResultFilter? appointmentResultFilter, int page, int pageSize, CancellationToken cancellationToken)
    {
        var set = _dbSet.Include(a => a.Appointment)
            .Where(a => a.Appointment.PatientId == patientId)
            .AsQueryable();

        return await FilterAsync(set, appointmentResultFilter, page, pageSize, cancellationToken);
    }
}