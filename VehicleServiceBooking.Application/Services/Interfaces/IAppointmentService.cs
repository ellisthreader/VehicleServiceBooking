using VehicleServiceBooking.Application.DTOs;

namespace VehicleServiceBooking.Application.Services.Interfaces;

public interface IAppointmentService
{
    Task<AppointmentDto> CreateAsync(CreateAppointmentRequest request);
    Task<AppointmentDto?> UpdateAsync(int id, UpdateAppointmentRequest request);
    Task<bool> CancelAsync(int id);
}
