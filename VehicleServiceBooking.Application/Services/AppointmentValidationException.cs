namespace VehicleServiceBooking.Application.Services;

public sealed class AppointmentValidationException(string message) : Exception(message);
