using CarWashFinancialSystem.Data;
using CarWashFinancialSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;

namespace CarWashFinancialSystem.Services
{
    public class TransactionService
    {
        private readonly AppDbContext _context;

        public TransactionService()
        {
            _context = new AppDbContext();
        }

        public ObservableCollection<Transaction> GetAllTransactions()
        {
            var transactions = _context.Transactions
                .Include(t => t.Service)
                .Include(t => t.Operator)
                .OrderByDescending(t => t.TransactionDate)
                .ToList();

            return new ObservableCollection<Transaction>(transactions);
        }

        public bool AddTransaction(int serviceId, decimal amount, string carType,
                                 string licensePlate, string paymentMethod, string notes, int? operatorId = null)
        {
            try
            {
                var transaction = new Transaction
                {
                    ServiceId = serviceId,
                    Amount = amount,
                    CarType = carType,
                    LicensePlate = licensePlate,
                    PaymentMethod = paymentMethod,
                    Notes = notes,
                    OperatorId = operatorId,
                    TransactionDate = DateTime.Now
                };

                _context.Transactions.Add(transaction);
                _context.SaveChanges();
                App.Logger.LogInfo($"Транзакция создана: {amount} руб, {carType}, {paymentMethod}", component: "Сервис транзакций");
                return true;
            }
            catch (Exception ex)
            {
                App.Logger.LogError($"Не удалось создать транзакцию: {ex.Message}", component: "Сервис транзакций");
                return false;
            }
        }

        public List<Transaction> GetTodayTransactions()
        {
            var today = DateTime.Today;
            return _context.Transactions
                .Include(t => t.Service)
                .Where(t => t.TransactionDate >= today)
                .ToList();
        }

        public decimal GetTodayRevenue()
        {
            var today = DateTime.Today;
            App.Logger.LogInfo($"Запрос сегодняшнего дохода: {_context.Transactions.Where(t => t.TransactionDate >= today).Sum(t => t.Amount)} руб",component: "Сервис транзакций");
            return _context.Transactions
                .Where(t => t.TransactionDate >= today)
                .Sum(t => t.Amount);
        }

        public int GetTodayTransactionsCount()
        {
            var today = DateTime.Today;
            return _context.Transactions
                .Count(t => t.TransactionDate >= today);
        }
    }
}