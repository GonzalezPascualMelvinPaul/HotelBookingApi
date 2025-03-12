using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Persistence;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class RoomRepositoryTests
{
    private ApplicationDbContext GetDatabaseContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()) 
            .Options;

        return new ApplicationDbContext(options);
    }

    [Fact]
    public async Task AddRoomAsync_Should_Add_New_Room()
    {
        var dbContext = GetDatabaseContext();
        var repository = new RoomRepository(dbContext);
        var room = new Room
        {
            RoomNumber = "101",
            Type = "Standard",
            Price = 100.00m,
            IsAvailable = true
        };

        await repository.AddRoomAsync(room);
        var rooms = await dbContext.Rooms.ToListAsync();

        Assert.Single(rooms);
        Assert.Equal("101", rooms[0].RoomNumber);
    }

    [Fact]
    public async Task GetVipRoomsAsync_Should_Return_Only_VIP_Rooms()
    {
        var dbContext = GetDatabaseContext();
        var repository = new RoomRepository(dbContext);

        var rooms = new List<Room>
        {
            new Room { RoomNumber = "101", Type = "Standard", Price = 100, IsAvailable = true },
            new Room { RoomNumber = "201", Type = "VIP", Price = 300, IsAvailable = true },
            new Room { RoomNumber = "301", Type = "VIP", Price = 400, IsAvailable = false }
        };

        await dbContext.Rooms.AddRangeAsync(rooms);
        await dbContext.SaveChangesAsync();

        var vipRooms = await repository.GetVipRoomsAsync();

        Assert.NotEmpty(vipRooms);
        Assert.All(vipRooms, room => Assert.Equal("VIP", room.Type));
    }
}
