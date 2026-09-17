using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Services.Interfaces;
using VehicleServiceBooking.Data.Models;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Application.Services;

public sealed class ServiceTypeService(IServiceTypeRepository repository) : IServiceTypeService
{
    public async Task<IReadOnlyList<ServiceTypeDto>> GetAllAsync() =>
        (await repository.GetAllAsync()).Select(Map).ToList();

    public async Task<ServiceTypeDto?> GetByIdAsync(int id)
    {
        var serviceType = await repository.GetByIdAsync(id);
        return serviceType is null ? null : Map(serviceType);
    }

    private static ServiceTypeDto Map(ServiceType serviceType) =>
        new(serviceType.Id, serviceType.Name, serviceType.Description, serviceType.Duration);
}
