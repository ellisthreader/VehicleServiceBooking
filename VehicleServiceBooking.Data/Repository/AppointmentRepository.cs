using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Data.DbContexts;
using VehicleServiceBooking.Data.Models;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Data.Repository;

public sealed class AppointmentRepository(DataContext context) : IAppointmentRepository
{
    public async Task<IReadOnlyList<Appointment>> GetAllAsync(int page, int pageSize)
    {
        if (page < 1)
            throw new ArgumentOutOfRangeException(nameof(page), "Page must be at least 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Page size must be at least 1.");

        return await context.Appointments
            .AsNoTracking()
            .Include(x => x.ServiceType)
            .OrderBy(x => x.ScheduledDate)
            .ThenBy(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public Task<Appointment?> GetByIdAsync(int id) =>
        context.Appointments.Include(x => x.ServiceType).SingleOrDefaultAsync(x => x.Id == id);

    public Task<bool> IsSlotTakenAsync(int serviceTypeId, DateTime scheduledDate) =>
        context.Appointments.AnyAsync(x => x.ServiceTypeId == serviceTypeId && x.ScheduledDate == scheduledDate);

    public async Task<Appointment> CreateAsync(Appointment appointment)
    {
        context.Appointments.Add(appointment);
        await context.SaveChangesAsync();
        return await GetByIdAsync(appointment.Id) ?? appointment;
    }

    public async Task<Appointment> UpdateAsync(Appointment appointment)
    {
        await context.SaveChangesAsync();
        return await GetByIdAsync(appointment.Id) ?? appointment;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var appointment = await context.Appointments.FindAsync(id);
        if (appointment is null) return false;
        context.Appointments.Remove(appointment);
        await context.SaveChangesAsync();
        return true;
    }
}
