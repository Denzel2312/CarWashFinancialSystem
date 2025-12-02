using Microsoft.EntityFrameworkCore;
using CarWashFinancialSystem.Models;

namespace CarWashFinancialSystem.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Service> Services { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<Expense> Expenses { get; set; }
        public DbSet<FinancialReport> FinancialReports { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(@"Server=(localdb)\mssqllocaldb;Database=CarWashFinancialDb;Trusted_Connection=True;");
        }

        public override int SaveChanges()
        {
            try
            {
                return base.SaveChanges();
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Ошибка сохранения базы данных: {ex.Message}",
                                   component: "База данных");
                throw;
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Создание начального администратора
            modelBuilder.Entity<User>().HasData(
                new User
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "admin123",
                    FullName = "Администратор системы",
                    Role = UserRole.Administrator
                }
            );

            modelBuilder.Entity<Service>().HasData(
                new Service { Id = 1, Name = "Экспресс-мойка", Price = 500, DurationMinutes = 15, Description = "Быстрая мойка" },
                new Service { Id = 2, Name = "Стандартная мойка", Price = 800, DurationMinutes = 30, Description = "Полная мойка" },
                new Service { Id = 3, Name = "Комплексная мойка", Price = 1200, DurationMinutes = 45, Description = "Мойка с химчисткой" }
            );
        }
    }
}