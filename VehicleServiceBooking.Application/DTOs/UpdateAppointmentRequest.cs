namespace VehicleServiceBooking.Application.DTOs;

public sealed record UpdateAppointmentRequest(string CustomerName, string Email, string Phone, string VehicleVin,
    int ServiceTypeId, DateTime ScheduledDate);
