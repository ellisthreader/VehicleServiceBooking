namespace VehicleServiceBooking.Application.DTOs;

public sealed record CreateAppointmentRequest(string CustomerName, string Email, string Phone, string VehicleVin,
    int ServiceTypeId, DateTime ScheduledDate);
