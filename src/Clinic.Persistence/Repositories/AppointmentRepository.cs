using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Filters;
using Clinic.Domain.Filters.Sorting;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Clinic.Persistence.Repositories;

public sealed class AppointmentRepository(ClinnicContext context) : IAppointmentRepository
{
    private readonly DbSet<Appointment> _dbSet = context.Set<Appointment>();

    public async Task<Appointment?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<PaginatedCollection<Appointment>> GetPaginatedAsync(AppointmentFilter? appointmentFilter,
        int page, int pageSize, CancellationToken cancellationToken)
    {
        var filter = _dbSet.AsQueryable();
        if (appointmentFilter is not null)
        {
            if (appointmentFilter.Ids is not null && appointmentFilter.Ids.Count != 0)
            {
                filter = filter.Where(a => appointmentFilter.Ids.Contains(a.Id));
            }

            if (appointmentFilter.DoctorIds is not null && appointmentFilter.DoctorIds.Count != 0)
            {
                filter = filter.Where(a => appointmentFilter.DoctorIds.Contains(a.DoctorId));
            }

            if (appointmentFilter.PatientIds is not null && appointmentFilter.PatientIds.Count != 0)
            {
                filter = filter.Where(a => appointmentFilter.PatientIds.Contains(a.PatientId));
            }

            if (appointmentFilter.ServiceIds is not null && appointmentFilter.ServiceIds.Count != 0)
            {
                filter = filter.Where(a => appointmentFilter.ServiceIds.Contains(a.ServiceId));
            }

            if (appointmentFilter.DateTimeStart is not null)
            {
                filter = filter.Where(a => a.DateTimeStart >= appointmentFilter.DateTimeStart);
            }

            if (appointmentFilter.DateTimeEnd is not null)
            {
                filter = filter.Where(a => a.DateTimeStart <= appointmentFilter.DateTimeEnd);
            }

            if (appointmentFilter.IsApprovedByClinic is not null)
            {
                filter = filter.Where(a => appointmentFilter.IsApprovedByClinic == a.IsApprovedByClinic);
            }

            if (appointmentFilter.IsApprovedByUser is not null)
            {
                filter = filter.Where(a => appointmentFilter.IsApprovedByUser == a.IsApprovedByUser);
            }

            if (appointmentFilter.Statuses is not null && appointmentFilter.Statuses.Count > 0)
            {
                filter = filter.Where(a => appointmentFilter.Statuses.Contains(a.Status));
            }

            filter = appointmentFilter.OrderDescending
                ? filter.OrderByDescending(GetSortProperty(appointmentFilter))
                : filter.OrderBy(GetSortProperty(appointmentFilter));
        }
        else
        {
            filter = filter.OrderByDescending(a => a.Id);
        }

        var isPatientFilterSpecified = appointmentFilter is not null &&
                                appointmentFilter.PatientIds is not null &&
                                appointmentFilter.PatientIds.Count > 0;

        if (isPatientFilterSpecified)
        {
            filter = filter.Where(o => appointmentFilter!.PatientIds!.Contains(o.PatientId));
        }

        var totalCount = await filter.CountAsync(cancellationToken);

        var filterResult = await filter.AsNoTracking()
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Appointment>(filterResult, page, pageSize, totalCount);
    }

    private static Expression<Func<Appointment, object>> GetSortProperty(AppointmentFilter appointmentFilter)
    {
        return appointmentFilter.SortColumn switch
        {
            AppointmentSortColumn.Id => service => service.Id,
            AppointmentSortColumn.DotorId => service => service.DoctorId,
            AppointmentSortColumn.ServiceId => service => service.ServiceId,
            AppointmentSortColumn.DateTimeStart => service => service.DateTimeStart,
            AppointmentSortColumn.IsApprovedByUser => service => service.IsApprovedByUser,
            AppointmentSortColumn.IsApprovedByClinic => service => service.IsApprovedByClinic,
            AppointmentSortColumn.Status => service => service.Status,
            AppointmentSortColumn.PatientId => service => service.PatientId,
            _ => service => service.Id
        };
    }

    public Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Add(appointment);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Update(appointment);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Remove(appointment);
        return context.SaveChangesAsync(cancellationToken);
    }

    public Task<Appointment?> GetAsync(uint patientId,
        uint doctorId,
        uint serviceId,
        DateTime dateTimeStart,
        DateTime dateTimeEnd,
        CancellationToken cancellationToken)
    {
        return _dbSet.FirstOrDefaultAsync(a => a.DoctorId == doctorId &&
                                      a.PatientId == patientId &&
                                      a.ServiceId == serviceId &&
                                      a.DateTimeStart == dateTimeStart &&
                                      a.DateTimeEnd == dateTimeEnd &&
                                      (a.Status == AppointmentStatus.Confirmed ||
                                       a.Status == AppointmentStatus.WaitingUserConfirm ||
                                       a.Status == AppointmentStatus.WaitingClinicConfirm),
                                 cancellationToken);
    }

    public async Task<PaginatedCollection<Appointment>> GetPatientsAppointments(uint patientId, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var patientsRequests = _dbSet.Where(a => a.PatientId == patientId);

        var totalCount = await patientsRequests.CountAsync(cancellationToken);

        var collection = await patientsRequests.OrderByDescending(o => o.DateTimeStart)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Appointment>(collection, page, pageSize, totalCount);
    }

    public async Task<PaginatedCollection<Appointment>> GetDoctorsAppointments(uint doctorId, int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var patientsRequests = _dbSet.Where(a => a.DoctorId == doctorId);

        var totalCount = await patientsRequests.CountAsync(cancellationToken);

        var collection = await patientsRequests.OrderByDescending(o => o.DateTimeStart)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Appointment>(collection, page, pageSize, totalCount);
    }

    public Task<bool> ExistsApprovedWithPatientAndDatetimeAsync(uint patientId, DateTime dateTimeStart,
        DateTime dateTimeEnd,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(a => a.PatientId == patientId &&
                                    (dateTimeStart < a.DateTimeEnd && a.DateTimeStart < dateTimeEnd) &&
                                    a.Status == AppointmentStatus.Confirmed,
            cancellationToken);
    }

    public Task<bool> ExistsApprovedWithDoctorAndDatetimeAsync(uint doctorId, DateTime dateTimeStart,
        DateTime dateTimeEnd,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(a => a.DoctorId == doctorId &&
                                    (dateTimeStart < a.DateTimeEnd && a.DateTimeStart < dateTimeEnd) &&
                                    a.Status == AppointmentStatus.Confirmed,
            cancellationToken);
    }

    public async Task<IEnumerable<Appointment>> GetDoctorsAppointmentsOnDate(uint doctorId, DateTime date,
        CancellationToken cancellationToken, AppointmentStatus? appointmentStatus = null)
    {
        var set = _dbSet.Where(a => a.DoctorId == doctorId && a.DateTimeStart.Date == date.Date);
        if (appointmentStatus != null)
        {
            set = set.Where(a => a.Status == appointmentStatus);
        }

        return await set.ToListAsync(cancellationToken);
    }
}