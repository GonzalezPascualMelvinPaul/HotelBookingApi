using HotelBooking.Domain.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HotelBooking.Infrastructure.Repositories
{
    public interface IRoomRepository
    {
        Task<IEnumerable<Room>> GetAllRoomsAsync();
        Task<IEnumerable<Room>> GetVipRoomsAsync();
        Task<Room> GetRoomByIdAsync(int id);
        Task AddRoomAsync(Room room);
        Task UpdateRoomAsync(Room room);
        Task DeleteRoomAsync(int id);
    }
}
