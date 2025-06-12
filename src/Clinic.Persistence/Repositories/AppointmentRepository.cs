using Clinic.Domain.Abstractions;
using Clinic.Domain.Entities;
using Clinic.Domain.Repositories;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Persistence.Repositories;

public sealed class AppointmentRepository(ClinnicContext context) : IAppointmentRepository
{
    private readonly DbSet<Appointment> _dbSet = context.Set<Appointment>();

    public async Task<Appointment?> GetByIdAsync(uint id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync([id], cancellationToken);
    }

    public async Task<ICollection<Appointment>> GetAllAsync(CancellationToken cancellationToken,
        bool includeTracking = false)
    {
        return includeTracking
            ? await _dbSet.AsNoTracking().ToListAsync(cancellationToken)
            : await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<PaginatedCollection<Appointment>> GetPaginatedAsync(int page, int pageSize,
        CancellationToken cancellationToken)
    {
        var totalCount = await _dbSet.CountAsync(cancellationToken);

        var collection = await _dbSet.OrderBy(o => o.Id)
            .AsNoTracking()
            .OrderBy(o => o.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return new PaginatedCollection<Appointment>(collection, page, pageSize, totalCount);
    }

    public async Task AddAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Add(appointment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Update(appointment);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Appointment appointment, CancellationToken cancellationToken)
    {
        context.Remove(appointment);
        await context.SaveChangesAsync(cancellationToken);
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

    public async Task<ICollection<Appointment>> GetAppointmentsForSpecializationOnDate(DateTime date,
        uint specializationId, CancellationToken cancellationToken)
    {
        return await _dbSet.Include(a => a.Doctor)
            .Where(a => a.Doctor.SpecializationId == specializationId &&
                        a.IsApprovedByClinic && a.Status == AppointmentStatus.Confirmed)
            .ToListAsync(cancellationToken);
    }

    public Task<bool> ExistsApprovedWithPatientAndDatetimeAsync(uint patientId, DateTime dateTimeStart,
        DateTime dateTimeEnd,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(a => a.PatientId == patientId &&
                                    ((dateTimeStart <= a.DateTimeEnd && a.DateTimeStart <= dateTimeEnd) ||
                                     (dateTimeStart >= a.DateTimeStart && dateTimeEnd <= a.DateTimeEnd) ||
                                     (a.DateTimeStart >= dateTimeStart && a.DateTimeEnd <= dateTimeEnd)) &&
                                    a.Status == AppointmentStatus.Confirmed,
            cancellationToken);
    }

    public Task<bool> ExistsApprovedWithDoctorAndDatetimeAsync(uint doctorId, DateTime dateTimeStart,
        DateTime dateTimeEnd,
        CancellationToken cancellationToken)
    {
        return _dbSet.AnyAsync(a => a.DoctorId == doctorId &&
                                    ((dateTimeStart <= a.DateTimeEnd && a.DateTimeStart <= dateTimeEnd) ||
                                     (dateTimeStart >= a.DateTimeStart && dateTimeEnd <= a.DateTimeEnd) ||
                                     (a.DateTimeStart >= dateTimeStart && a.DateTimeEnd <= dateTimeEnd)) &&
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