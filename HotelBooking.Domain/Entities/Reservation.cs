using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace HotelBooking.Domain.Entities
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required] 
        public int RoomId { get; set; }

        [ForeignKey("RoomId")]
        public Room Room { get; set; }  

        [Required]
        public string GuestName { get; set; }

        [Required]
        public DateTime CheckInDate { get; set; }

        [Required]
        public DateTime CheckOutDate { get; set; }

        [Required] 
        public string UserId { get; set; }
    }
}