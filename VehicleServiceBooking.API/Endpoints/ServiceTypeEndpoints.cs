using VehicleServiceBooking.Application.Services.Interfaces;

namespace VehicleServiceBooking.Api.Endpoints;

public static class ServiceTypeEndpoints
{
    public static void AddServiceTypeEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/service-types").WithTags("Service types");
        group.MapGet("/", async (IServiceTypeService service) => Results.Ok(await service.GetAllAsync()))
            .WithName("GetServiceTypes").WithSummary("List available service types").Produces(200);
        group.MapGet("/{id:int}", async (int id, IServiceTypeService service) =>
        {
            var result = await service.GetByIdAsync(id);
            return result is null ? Results.NotFound() : Results.Ok(result);
        }).WithName("GetServiceType").WithSummary("Get a service type").Produces(200).Produces(404);
    }
}
