using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace CarWashFinancialSystem.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService()
        {
            _context = new AppDbContext();
        }

        public User Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password && u.IsActive);
            return user;
        }

        public bool RegisterUser(string username, string password, string fullName, UserRole role)
        {
            if (_context.Users.Any(u => u.Username == username))
                return false;

            var user = new User
            {
                Username = username,
                PasswordHash = password, // В реальном приложении используй хэширование!
                FullName = fullName,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public List<User> GetAllUsers()
        {
            return _context.Users.Where(u => u.IsActive).ToList();
        }
    }
}