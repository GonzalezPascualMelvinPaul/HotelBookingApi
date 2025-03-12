using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Api.DTOs
{
    public class ReservationDto
    {
        [Required]
        public int RoomId { get; set; }

        [Required]
        public string GuestName { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }
    }
}
