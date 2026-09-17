using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Data.DbContexts;
using VehicleServiceBooking.Data.Models;

namespace VehicleServiceBooking.Data.Seed;

public static class DataSeeder
{
    public static async Task SeedAsync(DataContext context)
    {

        if (!await context.ServiceTypes.AnyAsync())
        {
            context.ServiceTypes.AddRange(
                new ServiceType { Name = "Oil Change", Description = "Engine oil and filter replacement.", Duration = 30 },
                new ServiceType { Name = "Interim Service", Description = "Routine inspection and maintenance service.", Duration = 60 },
                new ServiceType { Name = "Full Service", Description = "Comprehensive annual vehicle service.", Duration = 120 });
            await context.SaveChangesAsync();
        }
    }
}
