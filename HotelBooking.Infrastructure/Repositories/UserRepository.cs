using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBooking.Domain.Entities;

namespace HotelBooking.Infrastructure.Repositories
{
    public class UserRepository
    {
        private readonly List<User> _users = new()
        {
            new User { Id = 1, Username = "admin", Email = "admin@example.com", PasswordHash = "admin123", Role = "Admin" },
            new User { Id = 2, Username = "user", Email = "user@example.com", PasswordHash = "user123", Role = "Client" }
        };

        public Task<User> GetUserByEmailAsync(string email)
        {
            return Task.FromResult(_users.FirstOrDefault(u => u.Email == email));
        }
    }
}
