using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Services.Interfaces;
using VehicleServiceBooking.Data.Models;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Application.Services;

public sealed class AppointmentService(IAppointmentRepository appointments, IServiceTypeRepository serviceTypes)
    : IAppointmentService
{
    private static readonly TimeSpan OpeningTime = TimeSpan.FromHours(8);
    private static readonly TimeSpan ClosingTime = TimeSpan.FromHours(17);

    public async Task<AppointmentDto> CreateAsync(CreateAppointmentRequest request)
    {
        var serviceType = await ValidateAppointmentAsync(request.CustomerName, request.Email, request.Phone,
            request.VehicleVin, request.ServiceTypeId, request.ScheduledDate, checkAvailability: true);
        var appointment = BuildAppointment(request, serviceType.Id);

        try { return Map(await appointments.CreateAsync(appointment)); }
        catch (DbUpdateException ex) when (IsUniqueSlotViolation(ex))
        { throw new AppointmentValidationException("That service slot is already booked."); }
    }

    public async Task<AppointmentDto?> UpdateAsync(int id, UpdateAppointmentRequest request)
    {
        var appointment = await appointments.GetByIdAsync(id);
        if (appointment is null) return null;
        var serviceType = await ValidateAppointmentAsync(request.CustomerName, request.Email, request.Phone,
            request.VehicleVin, request.ServiceTypeId, request.ScheduledDate,
            checkAvailability: appointment.ServiceTypeId != request.ServiceTypeId ||
                               appointment.ScheduledDate != request.ScheduledDate);
        ApplyChanges(appointment, request, serviceType.Id);

        try { return Map(await appointments.UpdateAsync(appointment)); }
        catch (DbUpdateException ex) when (IsUniqueSlotViolation(ex))
        { throw new AppointmentValidationException("That service slot is already booked."); }
    }

    public Task<bool> CancelAsync(int id) => appointments.DeleteAsync(id);

    private async Task<ServiceType> ValidateAppointmentAsync(string customerName, string email, string phone,
        string vin, int serviceTypeId, DateTime scheduledDate, bool checkAvailability)
    {
        if (string.IsNullOrWhiteSpace(customerName))
            throw new AppointmentValidationException("Customer name is required.");
        if (string.IsNullOrWhiteSpace(email))
            throw new AppointmentValidationException("Email is required.");
        if (string.IsNullOrWhiteSpace(phone))
            throw new AppointmentValidationException("Phone is required.");

        var normalizedVin = vin?.Trim() ?? string.Empty;
        if (normalizedVin.Length != 17)
            throw new AppointmentValidationException("VIN must be exactly 17 characters.");

        var serviceType = await serviceTypes.GetByIdAsync(serviceTypeId)
            ?? throw new AppointmentValidationException("The selected service type does not exist.");

        if (scheduledDate.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
            throw new AppointmentValidationException("Appointments are available Monday through Friday only.");

        if (scheduledDate.TimeOfDay.Ticks % TimeSpan.TicksPerHour != 0)
            throw new AppointmentValidationException("Appointments must start on an exact hour.");

        if (scheduledDate.TimeOfDay < OpeningTime || scheduledDate.TimeOfDay > ClosingTime)
            throw new AppointmentValidationException("Appointments must be scheduled between 08:00 and 17:00.");

        if (checkAvailability && await appointments.IsSlotTakenAsync(serviceTypeId, scheduledDate))
            throw new AppointmentValidationException("That service slot is already booked.");

        return serviceType;
    }

    private static Appointment BuildAppointment(CreateAppointmentRequest request, int serviceTypeId) =>
        new()
        {
            CustomerName = request.CustomerName.Trim(),
            Email = request.Email.Trim(),
            Phone = request.Phone.Trim(),
            VehicleVin = request.VehicleVin.Trim().ToUpperInvariant(),
            ServiceTypeId = serviceTypeId,
            ScheduledDate = request.ScheduledDate
        };

    private static void ApplyChanges(Appointment appointment, UpdateAppointmentRequest request, int serviceTypeId)
    {
        appointment.CustomerName = request.CustomerName.Trim();
        appointment.Email = request.Email.Trim();
        appointment.Phone = request.Phone.Trim();
        appointment.VehicleVin = request.VehicleVin.Trim().ToUpperInvariant();
        appointment.ServiceTypeId = serviceTypeId;
        appointment.ScheduledDate = request.ScheduledDate;
    }

    private static bool IsUniqueSlotViolation(DbUpdateException exception) =>
        exception.InnerException?.Message.Contains("UNIQUE", StringComparison.OrdinalIgnoreCase) == true;

    private static AppointmentDto Map(Appointment appointment) =>
        new(appointment.Id, appointment.CustomerName, appointment.Email, appointment.Phone, appointment.VehicleVin,
            appointment.ServiceTypeId,
            new ServiceTypeDto(appointment.ServiceType.Id, appointment.ServiceType.Name, appointment.ServiceType.Description,
                appointment.ServiceType.Duration), appointment.ScheduledDate);
}
