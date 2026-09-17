using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Services.Interfaces;

public interface IServiceTypeService
{
    Task<IReadOnlyList<ServiceTypeDto>> GetAllAsync();
    Task<ServiceTypeDto?> GetByIdAsync(int id);
}
