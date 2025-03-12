using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Repositories
{
    public interface IReservationRepository
    {
        Task<IEnumerable<Reservation>> GetAllReservationsAsync();
        Task<Reservation> GetReservationByIdAsync(int id);
        Task AddReservationAsync(Reservation reservation);
        Task UpdateReservationAsync(Reservation reservation);
        Task DeleteReservationAsync(int id);
    }
}
