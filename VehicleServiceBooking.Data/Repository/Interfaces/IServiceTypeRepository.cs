using VehicleServiceBooking.Data.Models;

namespace VehicleServiceBooking.Data.Repository.Interfaces;

public interface IServiceTypeRepository
{
    Task<IReadOnlyList<ServiceType>> GetAllAsync();
    Task<ServiceType?> GetByIdAsync(int id);
}
