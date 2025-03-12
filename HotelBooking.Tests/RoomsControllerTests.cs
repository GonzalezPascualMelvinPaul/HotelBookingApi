using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Api.Controllers;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class RoomsControllerTests
{
    [Fact]
    public async Task GetRooms_Should_Return_List_Of_Rooms()
    {
       
        var mockRepo = new Mock<IRoomRepository>();
        var rooms = new List<Room>
        {
            new Room { Id = 1, RoomNumber = "101", Type = "Standard", Price = 100, IsAvailable = true },
            new Room { Id = 2, RoomNumber = "102", Type = "VIP", Price = 200, IsAvailable = false }
        };

        mockRepo.Setup(repo => repo.GetAllRoomsAsync()).ReturnsAsync(rooms);

        var controller = new RoomsController(mockRepo.Object);

       
        var result = await controller.GetRooms();

       
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value); 

        var response = okResult.Value as Dictionary<string, object>;
        Assert.NotNull(response); 

        Assert.True(response.ContainsKey("rooms"), "Response does not contain key 'rooms'"); 

        var roomList = response["rooms"] as List<Room>; 
        Assert.NotNull(roomList);
        Assert.NotEmpty(roomList);
    }


    [Fact]
    public async Task GetVipRooms_Should_Return_Only_VIP_Rooms()
    {
        
        var mockRepo = new Mock<IRoomRepository>();
        var vipRooms = new List<Room>
        {
            new Room { Id = 2, RoomNumber = "102", Type = "VIP", Price = 200, IsAvailable = true },
            new Room { Id = 3, RoomNumber = "103", Type = "VIP", Price = 250, IsAvailable = false }
        };

        mockRepo.Setup(repo => repo.GetVipRoomsAsync()).ReturnsAsync(vipRooms);

        var controller = new RoomsController(mockRepo.Object);

      
        var result = await controller.GetVipRooms();

      
        var okResult = Assert.IsType<OkObjectResult>(result);
        Assert.NotNull(okResult.Value); 

        var response = okResult.Value as Dictionary<string, object>;
        Assert.NotNull(response); 

        Assert.True(response.ContainsKey("rooms"), "Response does not contain key 'rooms'"); 

        var roomList = response["rooms"] as List<Room>; 
        Assert.NotNull(roomList);
        Assert.NotEmpty(roomList);
        Assert.All(roomList, room => Assert.Equal("VIP", room.Type));
    }


}
