using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CarWashFinancialSystem.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService()
        {
            _context = new AppDbContext();
        }

        // Существующие методы...
        public User Authenticate(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Username == username && u.PasswordHash == password && u.IsActive);
            return user;
        }

        public bool RegisterUser(string username, string password, string fullName, UserRole role)
        {
            if (_context.Users.Any(u => u.Username == username))
            {
                App.Logger.LogWarning($"Имя пользователя уже существует: {username}", user: username, component: "Система пользователей");
                return false;
            }
            var user = new User
            {
                Username = username,
                PasswordHash = password, // В реальном приложении используй хэширование!
                FullName = fullName,
                Role = role
            };

            _context.Users.Add(user);
            _context.SaveChanges();
            App.Logger.LogInfo($"Пользователь зарегистрирован: {username}", user: username, component: "Система пользователей");
            return true;
        }

        public ObservableCollection<User> GetAllUsers()
        {
            return new ObservableCollection<User>(_context.Users.Where(u => u.IsActive).ToList());
        }

        // Новые методы для управления пользователями
        public bool UpdateUser(int userId, string username, string fullName, UserRole role, bool isActive)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                // Проверяем уникальность username (кроме текущего пользователя)
                if (_context.Users.Any(u => u.Username == username && u.Id != userId))
                    return false;

                user.Username = username;
                user.FullName = fullName;
                user.Role = role;
                user.IsActive = isActive;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteUser(int userId)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                // Мягкое удаление
                user.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool ChangePassword(int userId, string newPassword)
        {
            try
            {
                var user = _context.Users.Find(userId);
                if (user == null) return false;

                user.PasswordHash = newPassword; // В реальном приложении хэшируй пароль!
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public User GetUserById(int userId)
        {
            return _context.Users.FirstOrDefault(u => u.Id == userId && u.IsActive);
        }

        public int GetUsersCount()
        {
            return _context.Users.Count(u => u.IsActive);
        }

        public int GetUsersCountByRole(UserRole role)
        {
            return _context.Users.Count(u => u.IsActive && u.Role == role);
        }
    }
}