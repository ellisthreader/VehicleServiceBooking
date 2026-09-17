using Moq;
using VehicleServiceBooking.Application.DTOs;
using VehicleServiceBooking.Application.Services;
using VehicleServiceBooking.Data.Models;
using VehicleServiceBooking.Data.Repository.Interfaces;

namespace VehicleServiceBooking.Tests;

[FixtureLifeCycle(LifeCycle.InstancePerTestCase)]
public class AppointmentServiceTests
{
    private readonly Mock<IAppointmentRepository> appointments = new();
    private readonly Mock<IServiceTypeRepository> serviceTypes = new();
    private readonly ServiceType oilChange = new() { Id = 1, Name = "Oil Change", Description = "Oil", Duration = 30 };

    private AppointmentService CreateService()
    {
        serviceTypes.Setup(x => x.GetByIdAsync(1)).ReturnsAsync(oilChange);
        return new AppointmentService(appointments.Object, serviceTypes.Object);
    }

    private static CreateAppointmentRequest ValidRequest(DateTime? date = null) =>
        new("Jane Doe", "jane@example.com", "0123456789", "WVWZZZ1JZXW000001", 1,
            date ?? new DateTime(2027, 1, 4, 9, 0, 0));

    [Test]
    public void CreateAsync_ShouldThrow_WhenVINIsNot17Characters() =>
        Assert.ThrowsAsync<AppointmentValidationException>(() =>
            CreateService().CreateAsync(ValidRequest() with { VehicleVin = "SHORT" }));

    [Test]
    public void CreateAsync_ShouldThrow_WhenSlotIsOutsideBusinessHours() =>
        Assert.ThrowsAsync<AppointmentValidationException>(() =>
            CreateService().CreateAsync(ValidRequest(new DateTime(2027, 1, 4, 19, 0, 0))));

    [Test]
    public void CreateAsync_ShouldThrow_WhenSlotIsOnAWeekend() =>
        Assert.ThrowsAsync<AppointmentValidationException>(() =>
            CreateService().CreateAsync(ValidRequest(new DateTime(2027, 1, 9, 9, 0, 0))));

    [Test]
    public void CreateAsync_ShouldThrow_WhenSlotIsAlreadyTaken()
    {
        appointments.Setup(x => x.IsSlotTakenAsync(1, It.IsAny<DateTime>())).ReturnsAsync(true);
        Assert.ThrowsAsync<AppointmentValidationException>(() => CreateService().CreateAsync(ValidRequest()));
    }

    [Test]
    public async Task CreateAsync_ShouldReturnAppointmentDto_WhenRequestIsValid()
    {
        appointments.Setup(x => x.IsSlotTakenAsync(1, It.IsAny<DateTime>())).ReturnsAsync(false);
        appointments.Setup(x => x.CreateAsync(It.IsAny<Appointment>())).ReturnsAsync((Appointment a) =>
        {
            a.Id = 42; a.ServiceType = oilChange; return a;
        });
        var result = await CreateService().CreateAsync(ValidRequest());
        Assert.That(result.Id, Is.EqualTo(42));
        Assert.That(result.VehicleVin, Is.EqualTo("WVWZZZ1JZXW000001"));
        Assert.That(result.ServiceType.Name, Is.EqualTo("Oil Change"));
    }

    [Test]
    public async Task CancelAsync_ShouldReturnFalse_WhenAppointmentDoesNotExist()
    {
        appointments.Setup(x => x.DeleteAsync(99)).ReturnsAsync(false);
        Assert.That(await CreateService().CancelAsync(99), Is.False);
    }

    [Test]
    public async Task CancelAsync_ShouldReturnTrue_WhenAppointmentExists()
    {
        appointments.Setup(x => x.DeleteAsync(1)).ReturnsAsync(true);
        Assert.That(await CreateService().CancelAsync(1), Is.True);
    }

    [Test]
    public void CreateAsync_ShouldThrow_WhenSlotIsNotOnAnExactHour() =>
        Assert.ThrowsAsync<AppointmentValidationException>(() =>
            CreateService().CreateAsync(ValidRequest(new DateTime(2027, 1, 4, 9, 30, 0))));

    [Test]
    public async Task UpdateAsync_ShouldNotCheckAvailability_WhenSlotIsUnchanged()
    {
        var existing = new Appointment { Id = 7, CustomerName = "Old", Email = "old@example.com", Phone = "1", VehicleVin = "WVWZZZ1JZXW000001", ServiceTypeId = 1, ServiceType = oilChange, ScheduledDate = new DateTime(2027, 1, 4, 9, 0, 0) };
        appointments.Setup(x => x.GetByIdAsync(7)).ReturnsAsync(existing);
        appointments.Setup(x => x.UpdateAsync(It.IsAny<Appointment>())).ReturnsAsync((Appointment a) => a);
        var result = await CreateService().UpdateAsync(7, new UpdateAppointmentRequest("New", "new@example.com", "2", "WVWZZZ1JZXW000001", 1, existing.ScheduledDate));
        Assert.That(result, Is.Not.Null);
        appointments.Verify(x => x.IsSlotTakenAsync(It.IsAny<int>(), It.IsAny<DateTime>()), Times.Never);
    }
}
