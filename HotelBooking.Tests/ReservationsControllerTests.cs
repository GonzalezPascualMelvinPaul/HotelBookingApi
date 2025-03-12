using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelBooking.Api.Controllers;
using HotelBooking.Api.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

public class ReservationsControllerTests
{
    [Fact]
    public async Task CreateReservation_Should_Return_CreatedAtAction()
    {
        var mockRepo = new Mock<IReservationRepository>();
        var controller = new ReservationsController(mockRepo.Object);

        var reservationDto = new ReservationDto
        {
            RoomId = 1,
            GuestName = "Carlos López",
            CheckInDate = DateTime.UtcNow,
            CheckOutDate = DateTime.UtcNow.AddDays(2)
        };

        var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user-123")
        }, "mock"));

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext { User = user }
        };

        var result = await controller.CreateReservation(reservationDto);

        var actionResult = Assert.IsType<CreatedAtActionResult>(result);
        Assert.Equal(201, actionResult.StatusCode);
    }
}
