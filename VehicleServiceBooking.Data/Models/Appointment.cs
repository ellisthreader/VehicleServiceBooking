namespace VehicleServiceBooking.Data.Models;

public class Appointment
{
    public int Id { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string VehicleVin { get; set; } = string.Empty;
    public int ServiceTypeId { get; set; }
    public ServiceType ServiceType { get; set; } = null!;
    public DateTime ScheduledDate { get; set; }
}
