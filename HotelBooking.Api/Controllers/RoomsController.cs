using HotelBooking.Domain.Entities;
using HotelBooking.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        private readonly IRoomRepository _roomRepository;

        public RoomsController(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        [HttpGet]
        [Authorize(Policy = "PuedeVerHabitaciones")]
        public async Task<IActionResult> GetRooms()
        {
            var rooms = await _roomRepository.GetAllRoomsAsync();

            if (rooms == null || !rooms.Any())
                return NotFound(new { status = 404, message = "No hay habitaciones disponibles." });

            return Ok(new Dictionary<string, object>
            {
                { "status", 200 },
                { "message", "Lista de habitaciones obtenida exitosamente." },
                { "rooms", rooms.ToList() } 
            });
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "PuedeVerHabitaciones")]
        public async Task<ActionResult<Room>> GetRoom(int id)
        {
            var room = await _roomRepository.GetRoomByIdAsync(id);
            if (room == null) return NotFound();
            return Ok(room);
        }

        [HttpPost]
        [Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult> CreateRoom([FromBody] Room room)
        {
            await _roomRepository.AddRoomAsync(room);
            return CreatedAtAction(nameof(GetRoom), new { id = room.Id }, new
            {
                status = 201,
                message = "Habitación creada exitosamente",
                room
            });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult> UpdateRoom(int id, Room room)
        {
            if (id != room.Id) return BadRequest();
            await _roomRepository.UpdateRoomAsync(room);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "EsAdmin")]
        public async Task<ActionResult> DeleteRoom(int id)
        {
            await _roomRepository.DeleteRoomAsync(id);
            return NoContent();
        }

        [HttpGet("vip")]
        [Authorize(Policy = "VerHabitacionesVIP")]
        public async Task<IActionResult> GetVipRooms()
        {
            var vipRooms = await _roomRepository.GetVipRoomsAsync();

            if (vipRooms == null || !vipRooms.Any())
                return NotFound(new { status = 404, message = "No hay habitaciones VIP disponibles." });

            return Ok(new Dictionary<string, object>
            {
                { "status", 200 },
                { "message", "Lista de habitaciones VIP obtenida exitosamente." },
                { "rooms", vipRooms.ToList() } 
            });
        }

    }
}
