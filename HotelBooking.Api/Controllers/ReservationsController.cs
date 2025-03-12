using HotelBooking.Api.DTOs;
using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace HotelBooking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        private readonly IReservationRepository _reservationRepository;

        public ReservationsController(IReservationRepository reservationRepository)
        {
            _reservationRepository = reservationRepository;
        }

        [HttpGet]
        [Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult<IEnumerable<Reservation>>> GetReservations()
        {
            return Ok(await _reservationRepository.GetAllReservationsAsync());
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult<Reservation>> GetReservation(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (reservation == null) return NotFound();
            return Ok(reservation);
        }

        [HttpPost]
        [Authorize(Policy = "PuedeReservar")]
        public async Task<IActionResult> CreateReservation([FromBody] ReservationDto reservationDto)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized(new { status = 401, message = "No estás autenticado." });

            var reservation = new Reservation
            {
                RoomId = reservationDto.RoomId,
                GuestName = reservationDto.GuestName,
                CheckInDate = reservationDto.CheckInDate,
                CheckOutDate = reservationDto.CheckOutDate,
                UserId = userId 
            };

            await _reservationRepository.AddReservationAsync(reservation);

            return CreatedAtAction(nameof(CreateReservation), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "EsAdmin")]
        public async Task<IActionResult> UpdateReservation(int id, [FromBody] ReservationDto reservationDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var existingReservation = await _reservationRepository.GetReservationByIdAsync(id);
            if (existingReservation == null)
                return NotFound(new { message = "Reserva no encontrada" });

            existingReservation.RoomId = reservationDto.RoomId;
            existingReservation.GuestName = reservationDto.GuestName;
            existingReservation.CheckInDate = reservationDto.CheckInDate;
            existingReservation.CheckOutDate = reservationDto.CheckOutDate;

            await _reservationRepository.UpdateReservationAsync(existingReservation);

            return Ok(new { message = "Reserva actualizada exitosamente" });
        }

        

        [HttpDelete("{id}")]
        [Authorize(Policy = "PuedeCancelar")]
        public async Task<IActionResult> CancelReservation(int id)
        {
            var reservation = await _reservationRepository.GetReservationByIdAsync(id);

            if (reservation == null)
                return NotFound(new { status = 404, message = "Reserva no encontrada." });

            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;

            if (reservation.UserId != userId && userRole != "Admin")
            {
                return Forbid(new AuthenticationProperties
                {
                    RedirectUri = "/error/no-autorizado"
                });
            }

            await _reservationRepository.DeleteReservationAsync(id);

            return Ok(new { status = 200, message = "Reserva cancelada exitosamente." });
        }

    }
}
