using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Application.Services.Interfaces;

namespace VehicleServiceBooking.Api.Endpoints;

public static class AppointmentEndpoints
{
    public static void AddAppointmentEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/appointments").WithTags("Appointments");
        group.MapPost("/", async (CreateAppointmentRequest request, IAppointmentService service) =>
        {
            try
            {
                var result = await service.CreateAsync(request);
                return Results.Created($"/appointments/{result.Id}", result);
            }
            catch (AppointmentValidationException ex) { return Results.BadRequest(new { error = ex.Message }); }
        }).WithName("CreateAppointment").WithSummary("Book an appointment").Produces<AppointmentDto>(201).Produces(400);

        group.MapPut("/{id:int}", async (int id, UpdateAppointmentRequest request, IAppointmentService service) =>
        {
            try
            {
                var result = await service.UpdateAsync(id, request);
                return result is null ? Results.NotFound() : Results.Ok(result);
            }
            catch (AppointmentValidationException ex) { return Results.BadRequest(new { error = ex.Message }); }
        }).WithName("UpdateAppointment").WithSummary("Update an appointment").Produces<AppointmentDto>(200).Produces(400).Produces(404);

        group.MapDelete("/{id:int}", async (int id, IAppointmentService service) =>
            await service.CancelAsync(id) ? Results.NoContent() : Results.NotFound())
            .WithName("CancelAppointment").WithSummary("Cancel an appointment").Produces(204).Produces(404);
    }
}
