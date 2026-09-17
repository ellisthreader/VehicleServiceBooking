namespace VehicleServiceBooking.Data.Models;

public class ServiceType
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Duration { get; set; }
    public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
}
