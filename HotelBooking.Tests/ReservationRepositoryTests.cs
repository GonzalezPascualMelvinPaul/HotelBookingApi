using System;
using System.Threading.Tasks;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Repositories;
using HotelBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class ReservationRepositoryTests
{
    private async Task<ApplicationDbContext> GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        var databaseContext = new ApplicationDbContext(options);
        await databaseContext.Database.EnsureCreatedAsync();

        return databaseContext;
    }

    [Fact]
    public async Task AddReservationAsync_Should_Add_New_Reservation()
    {
        var dbContext = await GetDatabaseContext();
        var repository = new ReservationRepository(dbContext);
        var reservation = new Reservation
        {
            RoomId = 1,
            GuestName = "Juan Pérez",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(2),
            UserId = "user-123"
        };

        await repository.AddReservationAsync(reservation);
        var reservations = await dbContext.Reservations.ToListAsync();

        Assert.Single(reservations);
        Assert.Equal("Juan Pérez", reservations[0].GuestName);
    }
}
