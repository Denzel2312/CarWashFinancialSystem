using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CarWashFinancialSystem.Services
{
    public class ServiceService
    {
        private readonly AppDbContext _context;

        public ServiceService()
        {
            _context = new AppDbContext();
        }

        public ObservableCollection<Service> GetAllServices()
        {
            var services = _context.Services.Where(s => s.IsActive).ToList();
            return new ObservableCollection<Service>(services);
        }

        public bool AddService(string name, decimal price, string description, int duration)
        {
            try
            {
                var service = new Service
                {
                    Name = name,
                    Price = price,
                    Description = description,
                    DurationMinutes = duration,
                    IsActive = true
                };

                _context.Services.Add(service);
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool UpdateService(int id, string name, decimal price, string description, int duration)
        {
            try
            {
                var service = _context.Services.Find(id);
                if (service == null) return false;

                service.Name = name;
                service.Price = price;
                service.Description = description;
                service.DurationMinutes = duration;

                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool DeleteService(int id)
        {
            try
            {
                var service = _context.Services.Find(id);
                if (service == null) return false;

                service.IsActive = false;
                _context.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public Service GetServiceById(int id)
        {
            return _context.Services.FirstOrDefault(s => s.Id == id && s.IsActive);
        }
    }
}