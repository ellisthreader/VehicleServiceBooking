using Microsoft.EntityFrameworkCore;
using VehicleServiceBooking.Data.Models;

namespace VehicleServiceBooking.Data.DbContexts;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    public DbSet<ServiceType> ServiceTypes => Set<ServiceType>();
    public DbSet<Appointment> Appointments => Set<Appointment>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ServiceType>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Name).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Description).HasMaxLength(500).IsRequired();
            entity.Property(x => x.Duration).IsRequired();
            entity.ToTable("ServiceTypes", table => table.HasCheckConstraint("CK_ServiceTypes_Duration_Positive", "Duration > 0"));
        });

        modelBuilder.Entity<Appointment>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.CustomerName).HasMaxLength(100).IsRequired();
            entity.Property(x => x.Email).HasMaxLength(254).IsRequired();
            entity.Property(x => x.Phone).HasMaxLength(30).IsRequired();
            entity.Property(x => x.VehicleVin).HasMaxLength(17).IsRequired();
            entity.Property(x => x.ScheduledDate).IsRequired();
            entity.HasOne(x => x.ServiceType)
                .WithMany(x => x.Appointments)
                .HasForeignKey(x => x.ServiceTypeId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(x => new { x.ServiceTypeId, x.ScheduledDate }).IsUnique();
        });
    }
}
