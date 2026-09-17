namespace VehicleServiceBooking.Application.DTOs;

public sealed record AppointmentDto(int Id, string CustomerName, string Email, string Phone, string VehicleVin,
    int ServiceTypeId, ServiceTypeDto ServiceType, DateTime ScheduledDate);
