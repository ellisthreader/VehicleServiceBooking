using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using VehicleServiceBooking.Data.DbContexts;
using VehicleServiceBooking.Data.Seed;
using VehicleServiceBooking.Data.Repository;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Data;

public static class DataModule
{
    public static void AddDataModule(IServiceCollection services, ConfigurationManager builderConfiguration)
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlite(builderConfiguration.GetConnectionString("DefaultConnection")
                ?? "Data Source=VehicleServiceBooking.db")
        );
        services.AddScoped<IServiceTypeRepository, ServiceTypeRepository>();
        services.AddScoped<IAppointmentRepository, AppointmentRepository>();
    }

    public static async Task SeedDb(IServiceProvider services)
    {
        var db = services.GetRequiredService<DataContext>();
        await db.Database.MigrateAsync();
        await DataSeeder.SeedAsync(db);
    }
}
