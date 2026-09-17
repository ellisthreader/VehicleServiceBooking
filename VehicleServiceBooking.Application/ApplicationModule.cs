using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Application.Services.Interfaces;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Application;

public static class ApplicationModule
{
    public static void AddApplicationModule(IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        services.AddScoped<IServiceTypeService, ServiceTypeService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
    }
}
