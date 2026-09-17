using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Data.DbContexts;
using VehicleServiceBooking.Data.Models;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Data.Repository;

public sealed class ServiceTypeRepository(DataContext context) : IServiceTypeRepository
{
    public async Task<IReadOnlyList<ServiceType>> GetAllAsync() =>
        await context.ServiceTypes.AsNoTracking().OrderBy(x => x.Name).ToListAsync();

    public Task<ServiceType?> GetByIdAsync(int id) =>
        context.ServiceTypes.AsNoTracking().SingleOrDefaultAsync(x => x.Id == id);
}
